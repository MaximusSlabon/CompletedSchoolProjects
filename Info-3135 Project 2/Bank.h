#pragma once
#include <iostream>
#include <fstream>
#include "Bank.h"
#include "Customer.h"
#include "Queue.h"
#include <iomanip>

class Bank {
public:
	Queue customers; // store all customers
	Queue approved; // queue for accepted for loan
	Queue denied; // queue for denied for loan

	Bank(); //default constructor

	void processCustomers();

	void printQueue(Queue *q);

	bool load(std::string fileName);

	bool save(std::string queue);
};
