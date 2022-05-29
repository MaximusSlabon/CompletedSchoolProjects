#pragma once
#include <iostream>
#include <string>
#include "patient.h"

//class will store helper methods for the patient project
// will include methods for display purposes

//print menu method
void print_menu();

//validate condition
bool validate_condition(int con);

bool validate_string(std::string s);

//intake and validate user data
//call PatientList's add method
Patient add_from_user();