#include <stdio.h>
#include <iostream>
#include "patientList.h"

int main() {

	PatientList myList = PatientList();//create a patient list

	bool run = true; // will be used to determine if the program should continue to run

	int user_choice = -1; // will hold input from user

	while (run) {
		print_menu(); //print options to user

		//collect and validate input
		std::cout << "Please select an option: ";
		std::cin >> user_choice;

		while (user_choice < 0 || user_choice > 6) {
			std::cout << "Please re enter valid number: ";
			std::cin >> user_choice;
		}

		std::cout << "" << std::endl; // buffer line

		switch (user_choice) { //great success
		case 1: //addpatient()
			myList.addPatient(add_from_user());
			//display list of patient with new addition
			myList.printQueue();
			//system pause, ask user to "press any button to  continue"
			system("pause");
			break;

		case 2: //processNext() //great success
			myList.processNextPatient();
			break;

		case 3: //displayQueue() //great success
			myList.printQueue();
			break;

		case 4: // viewHistory() //great success
			myList.printProcessed();
			break;

		case 5: // loadCSV() //great success
			myList.loadQueue();
			break;

		case 6: // saveCSV()
			myList.saveQueue();
			break;

		case 0: //quit
			std::cout << "**Session Ended**" << std::endl;
			run = false;
			break;
		}
		//std::cout << "-----------------" << std::endl << "=================" << std::endl << "-----------------" << std::endl;
	}

	return 0;
}