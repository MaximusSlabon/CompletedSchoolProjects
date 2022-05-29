#pragma once
#include <iostream>
#include <sstream>

class Patient {
public:
	std::string patientName;
	std::string  ailment;
	int severity;
	int timeCrit;
	int contag; // contgiousness
	int priorityScore; // will be used to determine position in queue

	Patient();
	~Patient();
	Patient(std::string _patientName, std::string _ailment, int _severity, int _timeCrit, int _contag);

	std::string toString();
	std::string toStringPlus();

	//operators
	bool operator < (Patient const& rhs) {
		return (this->priorityScore < rhs.priorityScore);
	};

	bool operator > (Patient const& rhs) {
		return (this->priorityScore > rhs.priorityScore);
	};

	bool operator == (Patient const& rhs) {
		return (this->priorityScore == rhs.priorityScore);
	};
};
