import pyqtgraph as pg
from pyqtgraph.Qt import QtWidgets, QtCore
import pandas as pd
import random
import datetime
import time

def state_to_brush(state):
    return brushes_list[states_list.index(state)]

def get_relative_time(cur_time):
    global init_ms_time
    
    return cur_time - init_ms_time

def init_patient_state_dict(patient_name, state, ms_time):
    patient_state_time[patient_name] = dict()
    patient_state_time[patient_name][state] = list()
    patient_state_time[patient_name][state].append(ms_time)
    
    """for state in states_list:
        patient_state_time[patient_name][state] = list()          # For each state, the relative times are stored
        patient_state_time[patient_name][state].append(ms_time)"""

app = QtWidgets.QApplication([])

patients_data = {
    "Paciente1": "En la entrada",
    "Paciente2": "En la entrada",
    "Paciente3": "En la entrada",
    "Paciente4": "En la entrada",
    "Paciente5": "En la entrada"
}

states_list = [
    "En la entrada",
    "En la sala de espera",
    "En tratamiento",
    "En su casa"
]

patient_to_row = dict()

brushes_list = ["b", "r", "y"]

bars = dict()
patient_state_time = dict()

# For each patient, stores last state and time (ms) received
patient_last_values = dict()

plot = pg.PlotWidget(title="Patient data")
plot.show()

init_ms_time = time.time() * 1000

for (i, (name, state)) in enumerate(patients_data.items()):
    
    bars[name] = list()
    init_patient_state_dict(name, state, 0)
    cur_brush = state_to_brush(state)
    patient_last_values[name] = (state, init_ms_time)
    patient_to_row[name] = i
    
    bar_graph = pg.BarGraphItem(
            x0 = [1],
            x1 = [1],
            height=[0.8],
            brush=cur_brush,
            y = [i],
            pen=pg.mkPen('k', width=2)
        )

    bars[name].append(bar_graph)
    plot.addItem(bar_graph)

def add_new_patient():
    
    new_index = len(patients_data)
    name = f"Paciente{new_index + 1}"
    
    bars[name] = list()
    patient_to_row[name] = new_index
    
    state = states_list[0]
    init_patient_state_dict(name, state, 0)
    cur_brush = state_to_brush(state)
    patient_last_values[name] = (state, init_ms_time)
    
    cur_time = time.time() * 1000
    cur_relative_time = get_relative_time(cur_time)
    
    bar_graph = pg.BarGraphItem(
            x0 = [cur_relative_time],
            x1 = [cur_relative_time],
            height=[0.8],
            brush=cur_brush,
            y = [new_index],
            pen=pg.mkPen('k', width=2)
        )

    bars[name].append(bar_graph)
    plot.addItem(bar_graph)
    
    patients_data[name] = state
    
    axis = plot.getAxis('left')
    axis.setTicks([[(idx, pname) for idx, pname in enumerate(patients_data.keys())]])

def get_next_state(last_state):
    state_index = states_list.index(last_state)
    return states_list[(state_index+1)%len(states_list)]

def get_previous_state(last_state):
    state_index = states_list.index(last_state)
    return states_list[(state_index-1)%len(states_list)]

def modifyValues():
    
    cur_time = time.time() * 1000
    cur_relative_time = get_relative_time(cur_time)
    
    for (i, (name, state)) in enumerate(patients_data.items()):
        last_state, last_time = patient_last_values[name]
        last_relative_time = get_relative_time(last_time)
        
        if state != states_list[-1] and random.uniform(1, 20) > 13:
            if get_next_state(last_state) == states_list[-1]:
                print(f"Paciente '{name}' se va a su casa")
            patients_data[name] = get_next_state(last_state)
        if state != states_list[0] and state != states_list[-1] and random.uniform(1, 20) > 16:
            patients_data[name] = get_previous_state(last_state)
    
    if random.uniform(1, 20) > 15 and len(patients_data)<8:
            add_new_patient()
    
def update():    
    
    modifyValues()
    
    active_patients = [act_name for act_name, act_state in patients_data.items()
                        if act_state != states_list[-1]]
    
    for name, state in patients_data.items():
        if state == states_list[-1]:
            for bar in bars[name]:
                plot.removeItem(bar)
            bars[name] = []
            
    patient_to_row.clear()
    for idx, name in enumerate(active_patients):
        patient_to_row[name] = idx
    
    for name in active_patients:
        new_y = patient_to_row[name]
    
        for bar in bars[name]:
            bar.setOpts(y=[new_y])
    
    axis = plot.getAxis('left')    
    axis.setTicks([[(i, name) for i, name in enumerate(active_patients)]])
    
    cur_time = time.time() * 1000
    cur_relative_time = get_relative_time(cur_time)
    
    for name in active_patients:
        
        state = patients_data[name]
        bar_idx = patient_to_row[name]
        
        cur_brush = state_to_brush(state)
        
        if name in patient_state_time:
            prev_state, prev_time  = patient_last_values[name]
            prev_relative_time  = get_relative_time(prev_time)
            
            if prev_state == state:     # No new bar segment needed
                bars[name][-1].setOpts(
                    x0 = [bars[name][-1].opts['x0'][0]],
                    x1 = [cur_relative_time]
                )
                patient_state_time[name][state].append(cur_relative_time)
                
                
            else:                                   # A new bar segment is required (state transition)
                last_end = bars[name][-1].opts['x1'][0]
                
                new_bar_graph = pg.BarGraphItem(
                    x0 = [last_end],
                    x1 = [cur_relative_time],
                    height=[0.8],
                    brush=cur_brush,
                    y = [bar_idx],
                    pen=pg.mkPen('k', width=2)
                )
                if state not in patient_state_time[name]:
                    patient_state_time[name][state] = list()
                    
                patient_state_time[name][state].append(cur_relative_time)
                patient_state_time[name][prev_state].append(cur_relative_time)    # The new state's time begins, and the
                                                                                    # previous one finishes

                bars[name].append(new_bar_graph)
                plot.addItem(new_bar_graph)
                patient_last_values[name] = (state, cur_time)
        
        else:
            init_patient_state_dict(name, 0)
            patient_last_values[name] = (state, cur_time)
            
            bar_graph = pg.BarGraphItem(
                    x0 = [cur_relative_time],
                    x1 = [cur_relative_time],
                    height=[0.8],
                    brush=cur_brush,
                    y = [bar_idx],
                    pen=pg.mkPen('k', width=2)
                )

            bars[name].append(bar_graph)
            plot.addItem(bar_graph)
        
    plot.setXRange(0, cur_relative_time + 1000)

plot.setLabel('left', 'Patients')
plot.setLabel('bottom', 'Time (ms)')
plot.showGrid(x=False, y=True)

timer = QtCore.QTimer()
timer.timeout.connect(update)
timer.start(1000)  # update every second

app.exec()

df = pd.DataFrame({p: {s: times for s, times in states.items()} for p, states in patient_state_time.items()})
df.to_csv("pruebas.csv")
print(df)