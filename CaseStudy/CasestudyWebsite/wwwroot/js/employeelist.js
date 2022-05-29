$(function () { //employeelist.js

    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); //this will return a promise so we await it 
                buildEmployeeList(payload);
                msg === "" ? //are we appending to an existing message
                    $("#status").text(" Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else if (response.status !== 404) { //probably some other client side error
                let problemJSON = await response.json();
                errorRtn(problemJSON, response.status);
            } else {//else 404 not found
                $("#status").text("no such path on server");
            }//else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; //getAll

    const clearModalFields = () => {
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstname").val("");
        $("#TextBoxLastname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        sessionStorage.removeItem("id");
        sessionStorage.removeItem("departmentId");
        sessionStorage.removeItem("timer");
    }; //clearModalFields

    $("#employeeList").click((e) => {
        clearModalFields();
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }//clicked elsewhere
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            data.map(employee => {
                if (employee.id === parseInt(id)) {
                    $("#TextBoxTitle").val(employee.title);
                    $("#TextBoxFirstname").val(employee.firstname);
                    $("#TextBoxLastname").val(employee.lastname);
                    $("#TextBoxPhone").val(employee.phoneno);
                    $("#TextBoxEmail").val(employee.email);
                    sessionStorage.setItem("id", employee.id);
                    sessionStorage.setItem("departmentId", employee.departmentId);
                    sessionStorage.setItem("timer", employee.timer);
                    $("#modalstatus").text("update data");
                    $("#myModal").modal("toggle");
                }//if
            }); //data.map
        } else {
            return false; // ignore if the clicked on heading or status
        }
    })//employeeListClick

    $("#updatebutton").click(async (e) => {
        try {
            //set up a new client side instance of employee
            stu = new Object();
            //pupulate 
            stu.title = $("#TextBoxTitle").val();
            stu.firstname = $("#TextBoxFirstname").val();
            stu.lastname = $("#TextBoxLastname").val();
            stu.phoneno = $("#TextBoxPhone").val();
            stu.email = $("#TextBoxEmail").val();
            //we stored these 3 earlier
            stu.id = parseInt(sessionStorage.getItem("id"));
            stu.departmentId = parseInt(sessionStorage.getItem("departmentId"));
            stu.timer = sessionStorage.getItem("timer");
            stu.picture64 = null;

            //send the updated back to the server asynch using PUT 
            let response = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(stu)
            });

            if (response.ok) // check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            } else if (response.status !== 404) { //probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { //404 not found
                $("#status").text("no such path on server")
            } //else
        } catch (error) {
            $("#status").text(error.message);
        } //try/catch
        $("#myModal").modal("toggle");
    }); //update button click 

    const buildEmployeeList = (data) => {
        $("#employeeList").empty();
        div = $(`<div class="list-group-item text-white bg-secondary row d-flex" id="status">employee Info</div>
                  <div class="list-group-item row d-flex text-center" id="heading" >
                  <div class="col-4 h4">Title</div>
                  <div class="col-4 h4">First</div>
                  <div class="col-4 h4">Last</div>
                </div>`);
        div.appendTo($("#employeeList"));
        sessionStorage.setItem("allemployees", JSON.stringify(data));
        data.map(stu => {
            btn = $(`<button class="list-group-item row d-flex" id="${stu.id}">`);
            btn.html(
                `<div class="col-4" id="employeetitle${stu.id}">${stu.title}</div>
                <div class="col-4" id="employeefirstname${stu.id}">${stu.firstname}</div>
                <div class="col-4" id="employeelastname${stu.id}">${stu.lastname}</div>`
            );
            btn.appendTo($("#employeeList"));
        });//map   
    }//buildemployeeList

    getAll(""); //first grab the data from the server
}); //jQuery ready method

//server was reached but server had a problem with the call
const errorRtn = (problemJson, status) => {
    if (status > 499) {
        $("#status").text("Problem server side, see debug console");
    } else {
        let keys = Object.keys(problemJson.errors)
        problem = {
            status: status,
            statusText: problemJson.error[keys[0][0]],
        };
        $("#status").text("Problem is cleint side, see browser console");
        console.log(problem);
    }
}
