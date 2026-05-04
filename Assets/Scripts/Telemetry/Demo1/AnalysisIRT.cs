using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisIRT : MonoBehaviour
{

    [System.Serializable]
    public class PatientData
    {
        public string name;
        public string doctor;
        public string state;
        public int triageLevel;

    }

    [System.Serializable]
    public class PatientDataList
    {
        public List<PatientData> patients;
    }

    private float timer = 0;

    [SerializeField]
    private float maxTime = 0.5f;

    private WebSocketClient wsc;

    private List<Patient> patientList;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        wsc = WebSocketClient.getSharedInstance();
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > maxTime){
            string jsonToSend = buildPatientsData();
            wsc.SendData(jsonToSend);
            timer = 0;
        }
        
    }

    string buildPatientsData()
    {
        PatientDataList patientsDataList = new PatientDataList();
        patientsDataList.patients = new List<PatientData> ();

        List<Patient> patientsList = gameManager.getPatientsList();

        foreach (Patient patient in patientsList)
        {
            PatientData data = new PatientData();
            data.name = patient.GetPatientName();
            data.triageLevel = patient.GetTriageLevel();
            data.state = getCurState(patient);
            data.doctor = patient.HospitalInfo.doctor;
            patientsDataList.patients.Add(data);
        }

        return JsonUtility.ToJson(patientsDataList);

    }

    string getCurState(Patient patient)
    {
        HospitalInfo patientInfo = patient.HospitalInfo;

        if (patientInfo.receptionistStart == -1 || patientInfo.receptionistEnd == -1) return "En la entrada";
        else if (patientInfo.nurseStart == -1 || patientInfo.nurseEnd == -1 || patientInfo.doctorStart == -1) return "En la sala de espera";
        else if (patientInfo.doctorStart != -1 && patientInfo.doctorEnd == -1) return "En la consulta";
        return "En su casa";
    }
}
