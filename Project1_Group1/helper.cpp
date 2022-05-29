#include "helper.h"

void print_menu() {
	std::cout << "============================================" << std::endl;
	std::cout << "Welcome to Fanshawe's College Medical Center" << std::endl;
	std::cout << "============================================" << std::endl;

	std::cout << "" << std::endl; // buffer line

	std::cout << "Please make a selection then press enter" << std::endl;
	std::cout << "1. Add a Patient" << std::endl;
	std::cout << "2. Process Next Patient in Queue" << std::endl;
	std::cout << "3. Display Queue" << std::endl;
	std::cout << "4. View Processed Patients History" << std::endl;
	std::cout << "5. Load Queue From External 'csv' File" << std::endl;
	std::cout << "6. Save Queue To External 'csv' File" << std::endl;
	std::cout << "0. Exit" << std::endl;
	std::cout << "" << std::endl; // buffer line
}

bool validate_condition(int con) {
	if (con < 1 || con > 10) {
		return false;
	} else {
		return true;
	}
}

bool validate_string(std::string s) {
	if (s == "") {
		return false;
	}
	else {
		return true;
	}
}

Patient add_from_user() {
	//temp vars to hold temp patient data
	std::string tName;
	std::string tAil;
	int tSev;
	int tTimeCrit;
	int tContag;

	getline(std::cin, tName); //flush cin

	std::cout << "Enter patient name: ";
	getline(std::cin, tName);

	while (validate_string(tName)) {
		std::cout << "Enter a name: ";
		std::cin >> tName;
	}

	std::cout << "Enter ailment: ";
	getline(std::cin, tAil);

	while (validate_string(tAil)) {
		std::cout << "Enter an ailment: ";
		std::cin >> tAil;
	}

	std::cout << "Enter severity (1-10): ";
	std::cin >> tSev;
	while (!validate_condition(tSev)) {
		std::cout << "Enter valid severity (1-10): ";
		std::cin >> tSev;
	}

	std::cout << "Enter time criticality (1-10): ";
	std::cin >> tTimeCrit;
	while (!validate_condition(tTimeCrit)) {
		std::cout << "Enter valid time criticality (1-10): ";
		std::cin >> tTimeCrit;
	}
	
	std::cout << "Enter contagiousness (1-10): ";
	std::cin >> tContag;
	while (!validate_condition(tContag)) {
		std::cout << "Enter valid contagiousness (1-10): ";
		std::cin >> tContag;
	}

	//take data, make new patient
	Patient tPatient(tName, tAil, tSev, tTimeCrit, tContag);

	std::cout << "" << std::endl; // buffer line

	return tPatient;
}
