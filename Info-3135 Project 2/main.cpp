#include "Bank.h"

void print_menu() {

	std::cout << "============================================" << std::endl;
	std::cout << "Welcome to the Bank" << std::endl;
	std::cout << "============================================" << std::endl;

	std::cout << "" << std::endl; // buffer line

	std::cout << "Please make a selection then press enter" << std::endl;
	std::cout << "1. Load RAW Customer Data" << std::endl;
	std::cout << "2. Process Loan Approvals" << std::endl;
	std::cout << "3. Display Approved Queue" << std::endl;
	std::cout << "4. Display Deiend Queue" << std::endl;
	std::cout << "5. Save Approved and Denied to 'csv' File" << std::endl;
	std::cout << "0. Save Queue To External 'csv' File" << std::endl;
	std::cout << "" << std::endl; // buffer line
}

int main() {

	Bank myBank = Bank();

	bool run = true; // will be used to determine if the program should continue to run
	bool dataLoaded = false; // will be used to determine if data has been loaded into the program

	int userChoice = -1; // will hold input from user

	std::string fileName = "bank-data.csv";
	while (run) {
		print_menu(); //print options to user

		//collect and validate input
		std::cout << "Please select an option: ";
		std::cin >> userChoice;

		while (userChoice < 0 || userChoice > 5) {
			std::cout << "Please re enter valid number: ";
			std::cin >> userChoice;
		}

		std::cout << "" << std::endl; // buffer line

		if (userChoice == 1) { // LOAD DATA
			dataLoaded = myBank.load(fileName);
			std::cout << "" << std::endl; // buffer line
		}

		if (userChoice == 0) { // EXIT, but graceful
			run = false;
		}

		if (dataLoaded) {

			switch (userChoice) {

			case 2: //process for loan
				myBank.processCustomers();
				std::cout << "" << std::endl; // buffer line
				break;

			case 3: //display approved
				myBank.printQueue(&myBank.approved);
				std::cout << "" << std::endl; // buffer line
				break;

			case 4: // display denied
				
				myBank.printQueue(&myBank.denied);
				std::cout << "" << std::endl; // buffer line
				break;

			case 5: // saveCSV()
				myBank.save("approved");
				myBank.save("denied");
				std::cout << "" << std::endl; // buffer line
				break;
			}
		}
		else {
			// "load data before performing operations" error message
		}
	}
	return 0;
}
