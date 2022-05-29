#pragma once

#include "Customer.h"

struct Node
{
	Node* previous;
	Node* next;

	Customer data;
};