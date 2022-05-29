#pragma once
#include "patient.h"

struct Node
{
	Node* previous;
	Node* next;

	Patient data;
};