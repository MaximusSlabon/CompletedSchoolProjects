#pragma once
#include "node.h"

class Queue {
protected:
	Node* begin_;
	Node* end_;

public:

	Queue() : begin_(nullptr), end_(nullptr) {}

	void enqueue(const Customer& item);

	bool dequeue();

	Customer get(const unsigned index);

	bool isEmpty();

	int size();

	void clearQueue();

	//Sorting
	bool sortQueue();

};
