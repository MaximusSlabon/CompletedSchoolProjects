#include "Customer.h"

Customer::Customer(int id, int age, std::string sex, std::string region, double income, bool married,
	int children, bool car, bool saveAccount, bool currentAccount, bool mortgage) {
    this->id = id;
    this->age = age;
    this->sex = sex;
    this->region = region;
    this->income = income;
    this->married = married;
    this->children = children;
    this->car = car;
    this->saveAccount = saveAccount;
    this->currentAccount = currentAccount;
    this->mortgage = mortgage;
    ageRisk = assignRandVal();
	sexRisk = assignRandVal();
	regionRisk = assignRandVal();
	incomeRisk = assignRandVal();
	marriedRisk = assignRandVal();
	childrenRisk = assignRandVal();
	carRisk = assignRandVal();
	saveAccountRisk = assignRandVal();
	currentAccountRisk = assignRandVal();
	mortgageRisk = assignRandVal();
    totalRisk = (ageRisk + sexRisk + regionRisk + incomeRisk + marriedRisk + childrenRisk + carRisk + saveAccountRisk + currentAccountRisk + mortgageRisk) / 10;
    isApproved = false; //default for now
}

int Customer::assignRandVal() {
		return rand() % 10 + 1;
	}