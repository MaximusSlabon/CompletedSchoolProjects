#include "patientList.h"

PatientList::PatientList() {
	patientQueue = Queue();
	patientProcessed = Queue();
}

Patient PatientList::get(int index) {
	return patientQueue.get(index);
}

void PatientList::printQueue() {
	if (!patientQueue.isEmpty()) {
		for (int i = 0; i < patientQueue.size(); i++) {
			std::cout << i + 1 << ". " << patientQueue.get(i).toString() << std::endl;
		}
		std::cout << std::endl;
	}
	else {
		//tell user queue is empty
		std::cout << "Patient Queue is empty" << std::endl;
	}
}

void PatientList::addPatient(Patient p) {
	patientQueue.enqueue(p);

	patientQueue.sortQueue();
}

bool PatientList::processNextPatient() {

	if (patientQueue.isEmpty()) {
		std::cout << "Patient queue is empty" << std::endl;
		return false;
	}

	//display the patient to be processed
	std::string pString = patientQueue.get(0).toString();
	std::cout << pString << std::endl;

	//add highest priority patient to processed history
	patientProcessed.enqueue(patientQueue.get(0));

	//remove patient from the queue
	patientQueue.dequeue();

	return true;
}

void PatientList::printProcessed() {
	if (!patientProcessed.isEmpty()) {

		std::cout << "Patients processed" << std::endl;
		std::cout << "" << std::endl;

		for (int i = patientProcessed.size() - 1; i >= 0; i--) {
			std::cout << (patientProcessed.size() - i) << ". " << patientProcessed.get(i).toStringPlus() << std::endl;
		}
		std::cout << std::endl;
	}
	else {
		//tell user queue is empty
		std::cout << "Patient processed queue is empty" << std::endl;
	}
}

//load and save

bool PatientList::loadQueue() {
	std::string fileName = "";

	std::cout << "Enter a filepath to load from: ";
	std::cin >> fileName;

	while (!validate_string(fileName)) {
		std::cout << "Enter a filepath to load from: ";
		std::cin >> fileName;
	}

	std::fstream patientFile(fileName);

	patientQueue.clearQueue();

	if (!patientFile.is_open())
	{
		std::cout << "File: " << fileName << " not found." << std::endl;
		return false;
	}

	std::string data; //current line of data pulled from csv

	while (std::getline(patientFile, data)) {

		//temp variables to hold data

		std::string tName = "";
		std::string  tAil = "";
		std::string tSev = "";
		std::string tTime = "";
		std::string tContag = "";

		int varCount = 0; //keep track of which variable we are building

		//parse data from currentPatientData, build variables
		for (int i = 0; i < data.length(); i++) {
			if (data[i] == ',') {
				varCount++;
			}
			else {
				switch (varCount) {
				case 0:
					tName += data[i];
					break;
				case 1:
					tAil += data[i];
					break;
				case 2:
					tSev += data[i];
					break;
				case 3:
					tTime += data[i];
					break;
				case 4:
					tContag += data[i];
					break;
				}
			}
		}
		//create new patient with temp variables, parse tSev, tTime and tContag to ints
		Patient tempP(tName, tAil, std::stoi(tSev), std::stoi(tTime), std::stoi(tContag));

		addPatient(tempP); // add patient to the patient queue
	}
	//message of success
	std::cout << "Patient data successfully loaded from file: " << fileName << std::endl;
	return true;
}

bool PatientList::saveQueue() {
	std::string fileName = "";

	std::cout << "Enter a filepath to save to: ";
	std::cin >> fileName;

	while (!validate_string(fileName)) {
		std::cout << "Enter a filepath to save to: ";
		std::cin >> fileName;
	}

	std::ofstream patientSave(fileName); //create and open a new file

	if (!patientSave.is_open()) {
		std::cout << "Unable to open file: " << fileName << std::endl;
		return false;
	}
	else {
		//loop through patient queue
		for (int i = 0; i < patientQueue.size(); i++) {
			//write data into file
			patientSave << patientQueue.get(i).patientName << ",";
			patientSave << patientQueue.get(i).ailment << ",";
			patientSave << patientQueue.get(i).severity << ",";
			patientSave << patientQueue.get(i).timeCrit << ",";
			patientSave << patientQueue.get(i).contag;
			patientSave << "\n";
		}

		std::cout << "Patient data successfully saved to file: " << fileName << std::endl;
		return true;
	}
}