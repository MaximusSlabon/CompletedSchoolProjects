#pragma once
#include <sstream>

class Customer {
public:
	int id;
	int age;
	std::string sex;
	std::string region;
	double income;
	bool married;
	int children;
	bool car;
	bool saveAccount;
	bool currentAccount;
	bool mortgage;

	//risks value 1-10
	int ageRisk;
	int sexRisk;
	int regionRisk;
	int incomeRisk;
	int marriedRisk;
	int childrenRisk;
	int carRisk;
	int saveAccountRisk;
	int currentAccountRisk;
	int mortgageRisk;

	//TOTAL RISK
	int totalRisk;

	//IS APPROVED
	bool isApproved;

	//Operators
	bool operator < (Customer const& rhs) {
		return (this->totalRisk < rhs.totalRisk);
	};

	bool operator > (Customer const& rhs) {
		return (this->totalRisk > rhs.totalRisk);
	};

	bool operator == (Customer const& rhs) {
		return (this->totalRisk == rhs.totalRisk);
	};

	Customer() = default;

	Customer(int id, int age, std::string sex, std::string region, double income, bool married,
		int children, bool car, bool save_account, bool current_account, bool mortgage);

private:
	int assignRandVal();
};