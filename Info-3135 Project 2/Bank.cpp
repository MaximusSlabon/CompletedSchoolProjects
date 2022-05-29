#include "Bank.h"

std::string convertBoolToString(bool b) {
	if (b) {
		return "YES";
	}
	else {
		return "NO";
	}

}

bool convertStringToBool(std::string s) {
	if ("YES") {
		return true;
	}
	else {
		return false;
	}
}

Bank::Bank() {
	customers = Queue();
	approved = Queue();
	denied = Queue();
}

void Bank::printQueue(Queue *q) {
	if (q->size() == 0)
		std::cout << "Queue is empty" << std::endl;
	else {
		std::cout << "ID    Age Sex    Region     Income Married Kids Car SavAcc CurrAcc Mortgage TotalRisk" << std::endl;
		for (int i = 0; i < q->size(); i++) {
			std::cout << q->get(i).id << " " << q->get(i).age << "  " << 
			std::setw(6) << q->get(i).sex << " " << std::setw(10) << q->get(i).region << " " << std::setw(5
			) << q->get(i).income <<
			"  " << convertBoolToString(q->get(i).married) << "     " << q->get(i).children << 
			"    " << convertBoolToString(q->get(i).car) << " " << convertBoolToString(q->get(i).saveAccount) <<
			"    " << convertBoolToString(q->get(i).currentAccount) << "     " << convertBoolToString(q->get(i).mortgage) << 
			"      " << q->get(i).totalRisk << std::endl;
		};
	}
}

void Bank::processCustomers() {
	for (int i = 0; i < customers.size(); i++) {
		if (customers.get(i).totalRisk < 5) {
			Customer temp = customers.get(i);
			temp.isApproved = true;
			approved.enqueue(temp);
		}
		else {
			denied.enqueue(customers.get(i));
		}
	}

	//sort the customers
	approved.sortQueue();
	denied.sortQueue();

	std::cout << "Customers have been processed" << std::endl;
}

bool Bank::load(std::string fileName) {
 	std::ifstream customerFile(fileName); 

	customers.clearQueue();
	
	if (!customerFile.is_open())
	{
		std::cout << "File: " << fileName << " not found." << std::endl;
		return false;
	}
	
	std::string data; //current line of data pulled from csv

	bool firstLine = true;

	while (std::getline(customerFile, data)) {
		
		//temp variables to hold data
		std::string tId = "";
		std::string tAge = "";
		std::string tSex = "";
		std::string tRegion = "";
		std::string tIncome = "";
		std::string tMarried = "";
		std::string tChildren = "";
		std::string tCar = "";
		std::string tSaving = "";
		std::string tCurrent = "";
		std::string tMortgage = "";

		int varCount = 0; //keep track of which variable we are building
		//skip the first line of data in csv
		if (!firstLine) {
			//parse data from currentPatientData, build variables
			for (int i = 0; i < data.length(); i++) {
				if (data[i] == ',') {
					varCount++;
				}
				else {
					switch (varCount) {
					case 0:
						tId += data[i];
						break;
					case 1:
						tAge += data[i];
						break;
					case 2:
						tSex += data[i];
						break;
					case 3:
						tRegion += data[i];
						break;
					case 4:
						tIncome += data[i];
						break;
					case 5:
						tMarried += data[i];
						break;
					case 6:
						tChildren += data[i];
						break;
					case 7:
						tCar += data[i];
						break;
					case 8:
						tSaving += data[i];
						break;
					case 9:
						tCurrent += data[i];
						break;
					case 10:
						tMortgage += data[i];
						break;
					}
				}
			}
			{
				using namespace std; //create new customer with temp variables
				Customer tempC(stoi(tId.substr(2)), stoi(tAge), tSex, tRegion, stoi(tIncome),
					convertStringToBool(tMarried), stoi(tChildren), convertStringToBool(tCar),
					convertStringToBool(tSaving), convertStringToBool(tCurrent), convertStringToBool(tMortgage));

				customers.enqueue(tempC); // add customer to the queue
			}
		}
		firstLine = false;
	}
	std::cout << "Customer data successfully loaded from file: " << fileName << std::endl;
	return true;
}

bool Bank::save(std::string saveName) {
	
	Queue* save;
	std::ofstream saveFile(saveName + ".csv"); //create and open a new file

	if (saveName == "approved") {
		save = &approved;
	}
	else {
		save = &denied;
	}

	if (!saveFile.is_open()) {
		std::cout << "Unable to open file: " << saveName << ".csv" << std::endl;
		return false;
	}
	else {
		//write the first line into the file
		saveFile << "id,age,sex,region,income,married,children,car,save_act,current_act,mortgage" << "\n";

		// loop through queue
		for (int i = 0; i < save->size(); i++) {
			//write data into the file
			saveFile << "ID" << save->get(i).id << ",";
			saveFile << save->get(i).age << ",";
			saveFile << save->get(i).sex << ",";
			saveFile << save->get(i).region << ",";
			saveFile << save->get(i).income << ",";
			saveFile << convertBoolToString(save->get(i).married) << ",";
			saveFile << save->get(i).children << ",";
			saveFile << convertBoolToString(save->get(i).car) << ",";
			saveFile << convertBoolToString(save->get(i).saveAccount) << ",";
			saveFile << convertBoolToString(save->get(i).currentAccount) << ",";
			saveFile << convertBoolToString(save->get(i).mortgage);
			saveFile << "\n";
		}
	}
	//clear the pointer
	save = nullptr;
	delete save;

	std::cout << "Customer data successfully saved to file" << std::endl;

	return true;
}