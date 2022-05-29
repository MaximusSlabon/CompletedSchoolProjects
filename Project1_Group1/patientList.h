#pragma once
#include "queue.h"
#include <fstream>
#include "helper.h"

class PatientList {
		
	Queue patientQueue; //list of patients in the queues
	Queue patientProcessed; //store history of patients processed

public:

	//constructor

	PatientList();

	//accessor method
	Patient get(int index);

	void addPatient(Patient p);

	bool processNextPatient();

	//print patient queue
	void printQueue();
	
	void printProcessed();

	bool loadQueue();

	bool saveQueue();
};