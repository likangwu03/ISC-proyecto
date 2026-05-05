from pyqtgraph.Qt import QtWidgets, QtCore
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware
import datetime, json, random, threading, time, uvicorn
import pandas as pd
import pyqtgraph as pg

class TelemetryAnalysis(QtCore.QObject):
    
    update_signal = QtCore.Signal()
    
    def __init__(self):
        
        super().__init__()
        
        self.app = qt_app
        
        self.update_signal.connect(self.update, QtCore.Qt.QueuedConnection)     # Runs update function in the GUI thread
        
        self.plot = pg.PlotWidget(title="Patients data")
        
        self.init_ms_time = None
        
        self.pending_data = None
        
        self.patients_data = dict()          # Patient name-state pairs
        
        self.patients_priority = dict()      # Patient name-priority pairs

        self.states_list = [
            "En la entrada",
            "En la sala de espera",
            "En la consulta",
            "En su casa"
        ]

        self.doctors_list = list()

        self.doctor_patient_dict = dict()

        self.patient_to_row = dict()

        self.brushes_list = ["b", "r", "y"]

        self.bars = dict()

        self.patient_state_time = dict()         # Stores the time each patient remains in each transition

        self.patient_assigned_doctors = dict()   # Stores the doctor(s) a certain patient may have been attended by

        self.doctor_patient_time = dict()        # Stores the timestamps each doctor has a patient assigned

        self.doctor_patient_acum = dict()        # Stores the total time each doctor has a patient assigned (required because of having to take into account transitions)

        self.patient_last_values = dict()        # For each patient, stores last state and time (ms) received
    
    def init_time(self, init_time):
        self.init_ms_time = init_time
    
    def state_to_brush(self, state):
        return self.brushes_list[self.states_list.index(state)]

    def get_relative_time(self, cur_time):
        return cur_time - self.init_ms_time

    def init_patient_state_time(self, patient_name, state, ms_time):
        self.patient_state_time[patient_name] = dict()
        self.patient_state_time[patient_name][state] = list()
        self.patient_state_time[patient_name][state].append(ms_time)

    def add_new_patient(self, priority, name):
        
        new_index = len(self.patients_data)
        
        self.bars[name] = list()
        self.patient_to_row[name] = new_index
        self.patients_priority[name] = priority
        
        state = self.states_list[0]
        
        cur_time = time.time() * 1000
        
        # First patient
        if self.init_ms_time is None:
            self.init_ms_time = cur_time

        rel_time = self.get_relative_time(cur_time)
        self.init_patient_state_time(name, state, rel_time)
        
        cur_brush = self.state_to_brush(state)
        self.patient_last_values[name] = (state, self.init_ms_time)
        
        cur_time = time.time() * 1000
        cur_relative_time = self.get_relative_time(cur_time)
        
        bar_graph = pg.BarGraphItem(
                x0 = [cur_relative_time],
                x1 = [cur_relative_time],
                height=[0.8],
                brush=cur_brush,
                y = [new_index],
                pen=pg.mkPen('k', width=2)
            )

        self.bars[name].append(bar_graph)
        self.plot.addItem(bar_graph)
        
        self.patients_data[name] = state
        
        axis = self.plot.getAxis('left')
        axis.setTicks([[(idx, pname) for idx, pname in enumerate(self.patients_data.keys())]])

    def get_cur_state(self, cur_state):
        state_index = self.states_list.index(cur_state)
        return self.states_list[(state_index)%len(self.states_list)]
    
    def get_next_state(self, cur_state):
        state_index = self.states_list.index(cur_state)
        return self.states_list[(state_index+1)%len(self.states_list)]

    def get_previous_state(self, cur_state):
        state_index = self.states_list.index(cur_state)
        return self.states_list[(state_index-1)%len(self.states_list)]
        
    def update(self):
        
        if self.init_ms_time is None or self.pending_data is None:
            if self.pending_data:                                  # If there is already data , the time has to be initialised (first package)
                self.init_ms_time = time.time() * 1000
            else:                                                   # Else, the graphic display ahs to wait to be a honest representation of the reality
                return
        
        if self.pending_data:
            self.updatePatientsInfo(self.pending_data)
            self.pending_data = None
        
        for name in list(self.bars.keys()):
            if self.patients_data.get(name) == self.states_list[-1]:
                for bar in self.bars[name]: 
                    self.plot.removeItem(bar)
                del self.bars[name]
        
        active_patients = [n for n in self.patients_data if n in self.bars]
        
        self.patient_to_row.clear()
        
        for idx, name in enumerate(active_patients):    # Bars positions have to be updated according to the current active patients
            self.patient_to_row[name] = idx
        
        for name in active_patients:
            new_y = self.patient_to_row[name]
        
            for bar in self.bars[name]:
                bar.setOpts(y=[new_y])
        
        axis = self.plot.getAxis('left')    
        axis.setTicks([[(i, name) for i, name in enumerate(active_patients)]])      # Left index (patient names for each bar) update
        
        cur_time = time.time() * 1000
        cur_relative_time = self.get_relative_time(cur_time)
        
        for name in active_patients:                                    # For each active patient, the graphic self.bars have to be updated
            
            state = self.patients_data[name]
            bar_idx = self.patient_to_row[name]
            
            cur_brush = self.state_to_brush(state)
            
            if name in self.patient_state_time:                                          # If the patient already has a graphic, the transition has to be
                prev_state, prev_time  = self.patient_last_values[name]                  # evaluated, so that it extends the current bar or appends a new
                prev_relative_time  = self.get_relative_time(prev_time)                  # one, depending on having or not a state transition
                
                if prev_state == state:                                             # No new bar segment needed
                    self.bars[name][-1].setOpts(x1=[cur_relative_time])
                    self.patient_state_time[name][state].append(cur_relative_time)
                    
                else:                                                               # A new bar segment is required (state transition)
                    
                    if not self.bars[name]:
                        last_end = cur_relative_time
                    else:
                        last_end = self.bars[name][-1].opts['x1'][0]
                    
                    new_bar_graph = pg.BarGraphItem(
                        x0 = [last_end],
                        x1 = [cur_relative_time],
                        height=[0.8],
                        brush=cur_brush,
                        y = [bar_idx],
                        pen=pg.mkPen('k', width=2)
                    )
                    if state not in self.patient_state_time[name]:
                        self.patient_state_time[name][state] = list()
                        
                    self.patient_state_time[name][state].append(cur_relative_time)
                    self.patient_state_time[name][prev_state].append(cur_relative_time)    # The new state's time begins, and the
                                                                                        # previous one finishes
                    self.bars[name].append(new_bar_graph)
                    self.plot.addItem(new_bar_graph)
            
            else:
                self.init_patient_state_time(name, cur_relative_time)
                
                bar_graph = pg.BarGraphItem(
                        x0 = [cur_relative_time],
                        x1 = [cur_relative_time],
                        height=[0.8],
                        brush=cur_brush,
                        y = [bar_idx],
                        pen=pg.mkPen('k', width=2)
                    )

                self.bars[name].append(bar_graph)
                self.plot.addItem(bar_graph)
            
            self.patient_last_values[name] = (state, cur_time)
            
        self.plot.setXRange(0, cur_relative_time + 1000)

    def start(self):

        self.plot.show()

        for (i, (name, state)) in enumerate(self.patients_data.items()):
            
            self.bars[name] = list()
            self.init_patient_state_time(name, state, 0)
            
            cur_brush = self.state_to_brush(state)
            
            self.patient_last_values[name] = (state, self.init_ms_time)
            self.patient_to_row[name] = i
            
            bar_graph = pg.BarGraphItem(
                    x0 = [1],
                    x1 = [1],
                    height=[0.8],
                    brush=cur_brush,
                    y = [i],
                    pen=pg.mkPen('k', width=2)
                )

            self.bars[name].append(bar_graph)
            self.plot.addItem(bar_graph)


        self.plot.setLabel('left', 'Patients')
        self.plot.setLabel('bottom', 'Time (ms)')
        self.plot.showGrid(x=False, y=True)

        timer = QtCore.QTimer()
        timer.timeout.connect(self.update)
        timer.start(500)  # update every second

        self.app.exec()
        
    def finish(self):

        final_time = self.get_relative_time(time.time() * 1000)
        for doc, pats in self.doctor_patient_time.items():
            for pat, times in pats.items():
                if self.patients_data.get(pat) == self.states_list[2]: # "En la consulta"
                    self.doctor_patient_acum[doc][pat] += final_time - times[-1]

        self.update()

        timeline_df = pd.DataFrame({p: {s: times for s, times in states.items()} for p, states in self.patient_state_time.items()})
        timeline_df
        timeline_df.to_csv("patients_timeline.csv")
        print(timeline_df)

        doctors_df = pd.DataFrame(self.doctor_patient_acum).transpose()
        doctors_df.to_csv("doctors_patients.csv")
        print(doctors_df)

        priorities_df = pd.DataFrame([self.patients_priority])
        priorities_df.index.name="Prioridad"
        priorities_df.to_csv("patients_priority.csv")
        
    def updatePatientsInfo(self, received_info):
        
        if self.init_ms_time is None:
            self.init_ms_time = time.time() * 1000
        
        received_patient_names = list()
        
        for patient_info in received_info["patients"]:
            name = patient_info["name"]
            priority = patient_info["triageLevel"]
            doctor = patient_info["doctor"]
            state = patient_info["state"]
            
            received_patient_names.append(name)
            
            if name not in self.patients_data:
                self.add_new_patient(priority, name)
            else:
                if name not in self.patient_state_time:
                    self.init_patient_state_time(name, state, 0)
            
            self.patients_data[name] = state
            self.patients_priority[name] = priority          # It starts at 0, so the program has to listen for changes
                
            if doctor not in self.doctors_list:              # If there is an assigned doctor, the state has to be "At the doctor's"
                self.doctors_list.append(doctor)
            
            if self.get_cur_state(state) == self.states_list[-2]:
                self.updateDoctorsTime(doctor, name)
                
        for name, state in self.patients_data.items():#.items():
            if name not in received_patient_names:                # The patient already went home
                self.patients_data[name] = self.states_list[-1]

    def updateDoctorsTime(self, doctor_name, patient_name):
        
        cur_time = time.time() * 1000
        cur_relative_time = self.get_relative_time(cur_time)
        
        if doctor_name not in self.doctor_patient_time:                     
            self.doctor_patient_time[doctor_name] = dict()
            self.doctor_patient_acum[doctor_name] = dict()
            
        if patient_name not in self.doctor_patient_time[doctor_name]:
            self.doctor_patient_time[doctor_name][patient_name] = list()
            self.doctor_patient_acum[doctor_name][patient_name] = 0
        
        if patient_name not in self.patient_assigned_doctors:
            self.patient_assigned_doctors[patient_name] = list()
        
        if doctor_name not in self.patient_assigned_doctors[patient_name]:
            self.patient_assigned_doctors[patient_name].append(doctor_name)
            
        self.doctor_patient_time[doctor_name][patient_name].append(cur_relative_time)
        
        if len(self.doctor_patient_time[doctor_name][patient_name])>1:
            patient_last_value = self.doctor_patient_time[doctor_name][patient_name][-2]
            self.doctor_patient_acum[doctor_name][patient_name] += cur_relative_time - patient_last_value
        else:
            self.doctor_patient_acum[doctor_name][patient_name] = 0

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

telemetryAnalysis = None
global init_time_set 

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):

        await websocket.accept()
        
        try:
            while True:
                
                data = await websocket.receive_text()

                parsed = json.loads(data)
                
                if parsed["patients"]:
                    
                    telemetryAnalysis.pending_data = parsed
                
        except WebSocketDisconnect:
            
            telemetryAnalysis.finish()

def run_api():
    uvicorn.run(app, host="0.0.0.0", port=8000, reload=False, workers=1)

if __name__ == "__main__":
    print("Hello world!")
    
    qt_app = QtWidgets.QApplication([])
    
    telemetryAnalysis = TelemetryAnalysis()
    
    api_thread = threading.Thread(target=run_api, daemon=True)
    api_thread.start()
    
    telemetryAnalysis.start()
    
    qt_app.exec()
