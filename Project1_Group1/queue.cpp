#include "queue.h"

void Queue::enqueue(const Patient& item)
{
	Node* node = new Node();
	node->data = item;

	//if queue is empty then the forst and last item become the new node
	if (begin_ == nullptr && end_ == nullptr)
	{
		begin_ = node;
		end_ = node;
		return;
	}
	//add node to the end of the queue

	node->previous = end_;
	end_->next = node;

	end_ = node;
}

bool Queue::dequeue() {
	if (begin_ == nullptr && end_ == nullptr) {
		//say it's empty
		return false;
	}
	else {
		Node* temp = begin_; //set a temp to the starting node
		begin_ = begin_->next; //dereferences begin, sets the second element in queue to new start

		//check if we removed last element in list
		if (begin_ == NULL) {
			end_ = NULL;
		}

		delete temp;
		return true;
	}
}

Patient Queue::get(const unsigned index) {
	size_t cursorComparer = 0;

	Node* cursor = new Node();

	cursor = begin_; // set cursor to start of queue

	while (cursorComparer++ != index) {
		cursor = cursor->next; //move along list
	}

	return cursor->data;
}

bool Queue::isEmpty() {
	if (begin_ == nullptr && end_ == nullptr) {
		return true;
	}
	else {
		return false;
	}
}

int Queue::size() {
	//check if list is empty
	if (begin_ == nullptr && end_ == nullptr) {
		return 0;
	}
	else {
		int counter = 0;

		Node* node = begin_;
		while (node != nullptr)
		{
			++counter;
			node = node->next;
		}

		return counter;
	}
}

void Queue::clearQueue() {

	Node* node = begin_;
	Node* temp;

	while (node != nullptr)
	{
		temp = node->next; // set pointer to next
		delete node; //delete current
		node = temp;
	}
	//set beginning and end to null pointers
	begin_ = nullptr;
	end_ = nullptr;
}

void swap(Patient& lhs, Patient& rhs) {
	Patient temp = lhs;

	lhs = rhs;
	rhs = temp;
}

bool Queue::sortQueue() {
	for (int i = 0; i < size() - 1; i++) {
		Node* cursor = begin_;
		int counter = 0;

		while (cursor->next != nullptr && counter < size() - 1 - i) {
			if (cursor->data < cursor->next->data) {
				swap(cursor->data, cursor->next->data);
			}

			cursor = cursor->next;
			counter++;
		}
	}
	return true;
}
