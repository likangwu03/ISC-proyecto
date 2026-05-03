import streamlit as st
import pandas as pd
import numpy as np
from matplotlib import pyplot as plt

st.set_page_config(page_title="Simulación médica: análisis interactivo", layout="wide", page_icon="https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.pixabay.com%2Fphoto%2F2017%2F05%2F15%2F23%2F47%2Fstethoscope-icon-2316460_1280.png&f=1&nofb=1&ipt=26df797828ccbb73935c176641e7f85491ca6af74d8ab1a4bae8f5396bc49b08")

if "timeline_df" not in st.session_state:
    st.session_state.timeline_df = pd.read_csv("patients_timeline.csv", index_col=0).transpose().fillna("[0]")

timeline_df = st.session_state.timeline_df

if "agg_results_df" not in st.session_state:
    st.session_state.agg_results_df = pd.read_csv("aggregated_results.csv", index_col=0).fillna(0)

agg_results_df = st.session_state.agg_results_df

if "docs_patients_df" not in st.session_state:
    st.session_state.docs_patients_df = pd.read_csv("doctors_patients.csv", index_col=0).fillna(0)

docs_patients_df = st.session_state.docs_patients_df

if "patients_priority_df" not in st.session_state:
    st.session_state.patients_priority_df = pd.read_csv("patients_priority.csv", index_col=0).transpose()

patients_priorities = st.session_state.patients_priority_df

cols = timeline_df.columns

patients = timeline_df.index

state_map = {
    "En la entrada": 0,
    "En la sala de espera": 1,
    "En la consulta": 2
}

states_number = len(list(state_map.keys()))
states_list = list(state_map.keys())

state_colors = {
    0: "#4dabf7",
    1: "#f59f00",
    2: "#51cf66"
}

if "doctors_time_priority" not in st.session_state:
    
    doctors_time_priority = dict()

    for doctor in docs_patients_df.index:
        doctors_time_priority[doctor] = dict()
        cur_doc_series = docs_patients_df.loc[doctor]
        doctors_time_priority[doctor]["totalTime"] = cur_doc_series.sum()
        cur_doc_patients = cur_doc_series[cur_doc_series!=0].index
        
        for patient in cur_doc_patients:
            patient_priority = f"Prioridad {patients_priorities.loc[patient][0]}"
            if patient_priority not in doctors_time_priority[doctor]:
                doctors_time_priority[doctor][patient_priority] = 1
            else:
                doctors_time_priority[doctor][patient_priority] += 1
    
    doctors_time_priority_df = pd.DataFrame(doctors_time_priority).fillna(0).transpose()
    doctors_time_priority_df["totalPatients"] = doctors_time_priority_df.iloc[:, 1:].sum(axis=1)
    st.session_state.doctors_time_priority = doctors_time_priority_df

doctors_time_priority = st.session_state.doctors_time_priority

def str_to_list(series):
    
    new_series = series.copy()
    
    for i in range(len(new_series)):
        
        lst = new_series.iloc[i]
        new_lst = lst.replace("[", "").replace("]", "").strip().split(",")
            
        for j in range(len(new_lst)):
            new_lst[j] = float(new_lst[j])

        new_series.iloc[i] = new_lst
        
    return new_series

st.image("https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.pixabay.com%2Fphoto%2F2017%2F05%2F15%2F23%2F47%2Fstethoscope-icon-2316460_1280.png&f=1&nofb=1&ipt=26df797828ccbb73935c176641e7f85491ca6af74d8ab1a4bae8f5396bc49b08", "", 50)

with st.container(horizontal=False, vertical_alignment="center"):
    
    st.markdown("<p style='color:#4dabf7; font-size: 30px; font-weight:bold; font-family: sans-serif; text-align: center; margin-bottom: 40px;'>DASHBOARD INTERACTIVO DE <br> LA SIMULACIÓN</p>",
                unsafe_allow_html=True)
    
    col1, col2 = st.columns(2)
    
    with col1:
        
        with st.container(horizontal=False):
            
            # First chart
            if "selected_patient" not in st.session_state:
                st.session_state.selected_patient = timeline_df.index[0]
            
            selected_patient = st.session_state.selected_patient
            
            patient_series = str_to_list(timeline_df.loc[selected_patient])

            times = []
            states = []

            for state, time_list in patient_series.items():
                for t in time_list:
                    times.append(t)
                    states.append(state_map[state])

            df_plot = pd.DataFrame({
                "time": times,
                "state": states
            })

            plt.style.use("seaborn-v0_8")

            fig, ax = plt.subplots(figsize=(10, 4))
            
            fig.subplots_adjust(
                left=0.08,
                right=0.98,
                top=1
            )

            for i in range(len(df_plot) - 1):
                x = [df_plot["time"][i], df_plot["time"][i+1]]
                y = [df_plot["state"][i], df_plot["state"][i]]

                ax.plot(x, y, color=state_colors[df_plot["state"][i]], linewidth=4)

            for i in range(len(df_plot) - 1):
                x = df_plot["time"][i+1]
                y = [df_plot["state"][i], df_plot["state"][i+1]]

                ax.plot([x, x], y, color="gray", linewidth=1.5, linestyle="--", alpha=0.6)

            ax.set_xlabel("Tiempo (ms)", fontsize=20, labelpad=20)
            ax.set_ylabel("Estado", fontsize=20, labelpad=20)

            ax.set_yticks(list(state_map.values()))
            ax.set_yticklabels(states_list)
            
            ax.tick_params(axis='x', labelsize=18)
            ax.tick_params(axis='y', labelsize=18)

            ax.set_title(f"Evolución temporal del paciente \n'{selected_patient}'", fontsize=28, pad=40, weight="bold")

            ax.grid(True, alpha=0.3)
            
            ax.legend(
                [],
                loc="upper center",
                bbox_to_anchor=(0.5, 1.1),
                ncol=3,
                frameon=False
            )

            st.pyplot(fig, width=550)
                
            patients = timeline_df.index
            
            with st.container(horizontal=True, horizontal_alignment="center"):

                selected_patient = st.selectbox("Selecciona un paciente",
                                            options=patients,
                                            width=200,
                                            key="selected_patient")
            
            st.space("medium")
            
            # Second chart
            
            with st.container(horizontal=True, horizontal_alignment="center"):
                
                st.space("large")
                st.space("medium")
                with st.container(horizontal=False):
                    doctors_list = docs_patients_df.index
                    
                    if "selected_doctor" not in st.session_state:
                        st.session_state.selected_doctor = doctors_list[0]

                    selected_doctor = st.session_state.selected_doctor
                    selected_doctor_series = docs_patients_df.loc[selected_doctor]
                    selected_doctor_series = selected_doctor_series[selected_doctor_series!=0]
                    
                    fig, ax = plt.subplots(figsize=(3.5, 3.5))

                    fig.subplots_adjust(
                        left=0.1,
                        right=0.9,
                        top=1,
                        bottom=0.1
                    )

                    total_time = selected_doctor_series.sum()

                    wedges, texts, autotexts = ax.pie(
                        selected_doctor_series,
                        labels=selected_doctor_series.index,
                        autopct='%1.1f%%',
                        pctdistance=0.75,
                        textprops={'fontsize': 9},
                        radius=0.8,
                        wedgeprops=dict(width=0.4)
                    )
                    
                    plt.setp(autotexts, size=10, weight="bold")

                    ax.text(
                        0, 0,
                        f"Tiempo total:\n{total_time:.0f} ms",
                        ha='center',
                        va='center',
                        fontsize=9,
                        weight='bold'
                    )

                    ax.set_title(
                        f"Desglose del doctor\n'{selected_doctor}' por pacientes",
                        fontsize=10,
                        weight="bold",
                        pad=5
                    )

                    st.pyplot(fig, width=350)
                    
                    with st.container(horizontal=True):
                        
                        st.space("large")
                        
                        selected_doctor = st.selectbox("Selecciona un doctor",
                                                    options=doctors_list,
                                                    width=200,
                                                    key="selected_doctor")
    
    with col2:
        
        with st.container(horizontal=False):
            
            # First chart    
            plt.style.use("seaborn-v0_8")
            fig, ax = plt.subplots(figsize=(10, 4))
            
            fig.subplots_adjust(
                left=0.08,
                right=0.98,
                top=1
            )
            
            if "patients_sort_state" not in st.session_state:
                st.session_state.patients_sort_state = states_list[0]
            
            patients_sort_state = st.session_state.patients_sort_state

            df_status_plot = agg_results_df.nlargest(5, patients_sort_state)[states_list].sort_values(patients_sort_state, ascending=False)
            top_patients_list = df_status_plot.index
            patients_number = len(df_status_plot)
            
            x = np.arange(patients_number)
            
            sep_arr = np.linspace(0.8, 1.2, states_number)
            width = 0.2
            
            for i in range(patients_number):
                for j in range(states_number):
                    ax.bar(x[i] + (j - (states_number - 1) / 2) * width,
                        df_status_plot.iloc[i].iloc[j],
                        width,
                        color=state_colors[j]
                    )

            ax.set_xticks(x)
            ax.set_xticklabels(top_patients_list)
            ax.set_xlabel("Paciente", fontsize=20, labelpad=20)
            ax.set_ylabel("Tiempo (ms)", fontsize=20, labelpad=20)
            
            ax.tick_params(axis='x', labelsize=18)
            ax.tick_params(axis='y', labelsize=18)
            
            ax.legend(
                states_list,
                loc="upper center",
                bbox_to_anchor=(0.5, 1.1),
                ncol=len(states_list),
                frameon=False,
                fontsize=14
            )
            
            ax.set_title(f"Top 5 pacientes que más han pasado en el estado \n'{patients_sort_state}'", fontsize=24, pad=40, weight="bold")
            
            ax.grid(True, alpha=0.3)

            st.pyplot(fig, width=550)
            
            with st.container(horizontal=True, horizontal_alignment="center"):

                patients_sort_state = st.radio("Selección de estado",
                                        label_visibility = "collapsed",
                                        horizontal=True,
                                        options=states_list,
                                        key="patients_sort_state")
            
            # Second chart
            
            st.space("medium")
            
            with st.container(horizontal=True):
            
                with st.container(horizontal=False):
                    
                    doctor_ratio = "totalTime" if "doctor_ratio" not in st.session_state else st.session_state.doctor_ratio
                    if "doctor_ratio" in st.session_state:
                        doctor_ratio = "totalTime" if doctor_ratio == "Tiempo total" else "totalPatients"
                    plt.style.use("seaborn-v0_8")
                    fig, ax = plt.subplots(figsize=(10, 4))
                    
                    fig.subplots_adjust(
                        left=0.08,
                        right=0.98,
                        top=1
                    )
                    
                    time_priority_chart_df = doctors_time_priority.sort_values(doctor_ratio, ascending=True)
                    
                    if doctor_ratio == "totalTime":
                        total_time_df = time_priority_chart_df.iloc[:, 0]
                        ax.barh(total_time_df.index, total_time_df)
                        ax.set_xlabel("Tiempo total", fontsize=20, labelpad=20)
                        ax.legend(
                            [],
                            loc="upper center",
                            bbox_to_anchor=(0.5, 1.1),
                            ncol=len(states_list),
                            frameon=False,
                            fontsize=14
                        )
                    
                    else:
                        patients_per_priority_df = time_priority_chart_df.iloc[:, 1:-1]
                        left = [0] * len(patients_per_priority_df)

                        for col in patients_per_priority_df.columns:
                            ax.barh(patients_per_priority_df.index, patients_per_priority_df[col], left=left,  label=f"Priority {col}")
                            left = [i + j for i, j in zip(left, patients_per_priority_df[col])]
                            
                        ax.legend(
                            patients_per_priority_df.columns,
                            loc="upper center",
                            bbox_to_anchor=(0.5, 1.1),
                            ncol=len(states_list),
                            frameon=True,
                            fontsize=14
                        )
                        ax.set_xlabel("Total pacientes", fontsize=20, labelpad=20)
                    
                    ax.set_ylabel("Doctor", fontsize=20, labelpad=20)
                    
                    ax.tick_params(axis='x', labelsize=18)
                    ax.tick_params(axis='y', labelsize=18)
            
                    ax.grid(True, alpha=0.3)
                    ax.set_title(f"Análisis de desempeño \n por doctor", fontsize=24, pad=40, weight="bold")

                    st.pyplot(fig, width=550)
                    
                    with st.container(horizontal=True, horizontal_alignment="center"):
                        
                        doctor_ratio = st.radio("Selección de ratio",
                                                label_visibility = "collapsed",
                                                horizontal=True,
                                                options=["Tiempo total", "Número de pacientes"],
                                                key="doctor_ratio")
            