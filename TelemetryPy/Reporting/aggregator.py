import pandas as pd
import copy

simulation_df = pd.read_csv("patients_timeline.csv", index_col=0).transpose().fillna("[0]")
priorities_df = pd.read_csv("patients_priority.csv").transpose()
doctors_df = pd.read_csv("doctors_patients.csv", index_col=0).transpose().fillna(0)

print(doctors_df)
print(pd.merge(simulation_df, doctors_df, left_index=True, right_index=True).head(4))

states_list = simulation_df.columns

treated_results = {patient: {state: 0 for state in states_list} for patient in simulation_df.index}

for p in range(len(simulation_df.index)):

    patient = simulation_df.iloc[p].copy()
    patient_name = simulation_df.index[p]

    for i in range(len(patient)):           # String list to float conversion
        
        patient.iloc[i] = patient.iloc[i].replace("[", "").replace("]", "").strip().split(",")
        
        for j in range(len(patient.iloc[i])):
            patient.iloc[i][j] = float(patient.iloc[i][j])

    patient_index_list = {i: 0 for i in range(len(states_list))}
    acum_state_index = {i: 0 for i in range(len(states_list))}

    visited_indexes = copy.deepcopy(patient)

    visited_indexes = patient.apply(lambda lst: [False] * len(lst))         # Visit dictionary (to avoid infinite loop when transitioning between states)

    cur_index = 0                                   # Set initial values
    patient_index_list[cur_index] = 1
    visited_indexes.iloc[cur_index][0] = True

    finish = False

    while not finish:                                                           # Iterate over all the values of each state, respecting the temporal and
                                                                                # the status order
        cur_subindex = patient_index_list[cur_index]
        
        cur_patient_time = patient.iloc[cur_index][cur_subindex]
        last_patient_time = patient.iloc[cur_index][cur_subindex-1]
        
        acum_state_index[cur_index] += cur_patient_time - last_patient_time     # Adds the current value's substract to the previous one
        visited_indexes.iloc[cur_index][cur_subindex] = True
        
        state_changed = False
        
        for i in range(len(patient)):

            if cur_patient_time in patient.iloc[i] and i != cur_index:       # If there is a state transition
                found_subindex = patient_index_list[i]
                
                if not visited_indexes.iloc[i][found_subindex]:             # If it has been visited , then it has to be ignored (due to
                                                                            # falling into an infinite loop)
                    cur_index = i
                    
                    if patient_index_list[cur_index] != len(patient.iloc[cur_index]) - 1:   # If there are more elements, it has to advance a position
                        patient_index_list[cur_index] += 1
                        
                    state_changed = True
                    break
        
        if not state_changed:
            
            if cur_subindex == len(patient.iloc[cur_index]) - 1:                # If the state does not change and it is the last index of the state's list,
                finish = True                                                   # then that means it has iterated over all the values for each state.
                
            else:
                patient_index_list[cur_index] += 1                              # Else, it advances to the next position of the same state
    
    for state_index, total_time in acum_state_index.items():                    # For each patient and state, the total time is set
        treated_results[patient_name][states_list[state_index]] = total_time
            
#print(treated_results)

df_results = pd.DataFrame(treated_results).transpose()

df_results["Prioridad"] = priorities_df

print(df_results)

df_results.to_csv("aggregated_results.csv")
