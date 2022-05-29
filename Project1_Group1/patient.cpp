#pragma once
// implementation of patient.h method stubs

#include "patient.h"

Patient::Patient() {
	patientName = "";
	ailment = "";
	severity = 0;
	timeCrit = 0;
	contag = 0;
	priorityScore = 0;
}

Patient::~Patient() {
	patientName = "";
	ailment = "";
	severity = 0;
	timeCrit = 0;
	contag = 0;
	priorityScore = 0;
}

Patient::Patient(std::string _pName, std::string _ailment, int _severity, int _timeCriticality, int _contag) {
	patientName = _pName;
	ailment = _ailment;
	severity = _severity;
	timeCrit = _timeCriticality;
	contag = _contag;
	priorityScore = severity + timeCrit + contag;
}

std::string Patient::toString() {
	std::string pat = "Name: ";
	pat += patientName;
	pat += ", Ailment: ";
	pat += ailment;
	pat += ", Priority Score: ";

	std::stringstream ssPrio;
	ssPrio << priorityScore;
	std::string tempPrio;
	ssPrio >> tempPrio;
	pat += tempPrio;

	return pat;
}

std::string Patient::toStringPlus() {
	std::string pat = "Name: ";
	pat += patientName;
	pat += ", Ailment: ";
	pat += ailment;
	pat += ", Priority Score: ";

	std::stringstream ssPrio;
	ssPrio << priorityScore;
	std::string tempPrio;
	ssPrio >> tempPrio;
	pat += tempPrio;

	std::stringstream ssSev;
	ssSev << severity;
	std::string tempSev;
	ssSev >> tempSev;
	pat += tempSev;

	std::stringstream ssTime;
	ssTime << timeCrit;
	std::string tempTime;
	ssTime >> tempTime;
	pat += tempTime;

	std::stringstream ssCont;
	ssCont << severity;
	std::string tempCont;
	ssCont >> tempCont;
	pat += tempCont;

	return pat;
}