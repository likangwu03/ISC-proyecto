import pyqtgraph as pg
from pyqtgraph.Qt import QtWidgets, QtCore
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
    "Paciente3": "En la entrada"
   # "Paciente4": "En la entrada",
   # "Paciente5": "En la entrada"
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

arrarra = 0

init_ms_time = time.time() * 1000

for (i, (name, state)) in enumerate(patients_data.items()):
    
    bars[i] = list()
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

    bars[i].append(bar_graph)
    plot.addItem(bar_graph)

def add_new_patient():
    new_index = len(patient_to_row)
    
    bars[new_index] = list()
    name = f"Paciente {new_index + 1}"
    
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

    bars[new_index].append(bar_graph)
    plot.addItem(bar_graph)
    
    patients_data[name] = state
    
    axis = plot.getAxis('left')
    axis.setTicks([[(idx, pname) for idx, pname in enumerate(patients_data.keys())]])

def get_next_state(last_state):
    state_index = states_list.index(last_state)
    return states_list[(state_index+1)%len(states_list)]

def modifyValues():
    
    cur_time = time.time() * 1000
    cur_relative_time = get_relative_time(cur_time)
    
    for (i, (name, state)) in enumerate(patients_data.items()):
        last_state, last_time = patient_last_values[name]
        last_relative_time = get_relative_time(last_time)
        
        if random.uniform(1, 20) > 19:
            print(f"Paciente '{name}' se mueve de estado")
            patients_data[name] = get_next_state(last_state)
    
    if random.uniform(1, 20) > 17:
            add_new_patient()
            print("newwww")
    
def update():    
    
    modifyValues()
    
    axis = plot.getAxis('left')
    axis.setTicks([[(i, name) for i, name in enumerate(patients_data.keys())]])    
    
    cur_time = time.time() * 1000
    cur_relative_time = get_relative_time(cur_time)
    
    for (i, (name, state)) in enumerate(patients_data.items()):
        
        if state == states_list[-1]:
            """for bar_i in bars[i]:
                last_bar = bar_i  # remove from our list
                plot.removeItem(last_bar)
            bars[i][-1].setOpts(
                    y=0.0001
                )
            patients_data.pop(name)
                
            last_bar = bars[i].pop()  # remove from our list
            plot.removeItem(last_bar)  # remove from plot
            
            axis.setTicks([[(i, name) for i, name in enumerate(patients_data.keys())]]) """
            continue
        
        cur_brush = state_to_brush(state)
        
        if name in patient_state_time:
            prev_state, prev_time  = patient_last_values[name]
            prev_relative_time  = get_relative_time(prev_time)
            
            if prev_state == state:     # No new bar segment needed
                bars[i][-1].setOpts(
                    x0 = [bars[i][-1].opts['x0'][0]],
                    x1 = [cur_relative_time]
                )
                patient_state_time[name][state].append(cur_relative_time)
                
                
            else:                                   # A new bar segment is required (state transition)
                last_end = bars[i][-1].opts['x1'][0]
                
                new_bar_graph = pg.BarGraphItem(
                    x0 = [last_end],
                    x1 = [cur_relative_time],
                    height=[0.8],
                    brush=cur_brush,
                    y = [i],
                    pen=pg.mkPen('k', width=2)
                )
                if state not in patient_state_time[name]:
                    patient_state_time[name][state] = list()
                    
                patient_state_time[name][state].append(cur_relative_time)
                patient_state_time[name][prev_state].append(cur_relative_time)    # The new state's time begins, and the
                                                                                    # previous one finishes

                bars[i].append(new_bar_graph)
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
                    y = [i],
                    pen=pg.mkPen('k', width=2)
                )

            bars[i].append(bar_graph)
            plot.addItem(bar_graph)
        
    plot.setXRange(0, cur_relative_time + 1000)

plot.setLabel('left', 'Patients')
plot.setLabel('bottom', 'Time (ms)')
plot.showGrid(x=False, y=True)

timer = QtCore.QTimer()
timer.timeout.connect(update)
timer.start(200)  # update every second

app.exec()


"""data = {
    "Barrel1": [0, 1.5, 2, 6],
    "Barrel2": [0.2, 0.9, 3, 7],
    "Barrel3": [1, 1.5, 4.2, 8]
}

brushes = ["b", "r", "y", "w"]
"""

"""
for i, (name, barrel_data) in enumerate(data.items()):
    
    print(type(barrel_data), barrel_data)
    
    bars[i] = list()
    
    for j in range(len(barrel_data)-1):
        
        start = barrel_data[j]
        end = barrel_data[j+1]
        brush = brushes[j%4]
        
        print(i, j, start, end, brush)
        
        bar_graph = pg.BarGraphItem(
            x0 = [start],
            x1 = [end],
            #width = [end-start-1],
            height=[0.8],
            brush=brush,
            y = [i],
            pen=pg.mkPen('k', width=2)
        )

        bars[i].append(bar_graph)
        plot.addItem(bar_graph)

def update():

    for name, barrel_data in data.items():
        barrel_data[-1] += pepe#.append(pepe)
    
    for ((name, barrel_data), (bar_number, graph_bar)) in zip(data.items(), bars.items()):
        start = barrel_data[-2]
        end = barrel_data[-1]
        graph_bar[-1].setOpts(
            x0 = [start],
            x1 = [end]
        )
    
    for (barrel_number, plt_bars) in bars.items():
        start = data
        plt_bars[-1].setOpts()
        
    for (bar, (name, barrel_data)) in zip(bars, data.items()):
        for j in range(len(barrel_data)-1):
        
            start = barrel_data[j]
            end = barrel_data[j+1]
            brush = brushes[j%4]
            
            print(i, j, start, end, brush)
            
            bar.setOpts(x0 = [start],
                x1 = [end])
    
    pepe += 3
        
"""