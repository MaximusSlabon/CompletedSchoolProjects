$(function () {

    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); //this will return a promise so we await it 
                buildemployeeList(payload);
                msg === "" ? //are we appending to an existing message
                    $("#status").text("employees Loaded") : $("#status").text(`${msg} - employees Loaded`);
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

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h>update employee</h4>");

        clearModalFields(); //clear everything

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
                $("#myModalLabel").text("Update");
            }//if
        }); //data.map
    };//setup for update

    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add employee</h4>");
        $("#myModal").modal("toggle");
        $("#modalstatus").text("enter data");
        $("#myModalLabel").text("Add");

        clearModalFields();
    }; //setup for add

    const clearModalFields = () => {
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstname").val("");
        $("#TextBoxLastname").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxEmail").val("");
        sessionStorage.removeItem("id");
        sessionStorage.removeItem("departmentId");
        sessionStorage.removeItem("timer");
        $("myModal").modal("toggle");
    }; //clearModalFields

    const add = async () => {
        try {
            //set up a new client side instance of employee
            stu = new Object();
            //pupulate 
            stu.title = $("#TextBoxTitle").val();
            stu.firstname = $("#TextBoxFirstname").val();
            stu.lastname = $("#TextBoxLastname").val();
            stu.phoneno = $("#TextBoxPhone").val();
            stu.email = $("#TextBoxEmail").val();

            stu.departmentId = 100; //hard code for now
            stu.id = -1;
            stu.timer = null;
            stu.picture64 = null;

            //send the updated back to the server async using POST
            let response = await fetch("api/employee", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
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
    }; //add

    const update = async () => {
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
    }; //update

    $("#actionbutton").click(() => {
        //button logic for generating right stuff
        $("#actionbutton").val() === "update" ? update() : add();
    });

    $("#employeeList").click((e) => {
        clearModalFields();
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }//clicked elsewhere

        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false; // ignore if the clicked on heading or status
        }
    })//employeeListClick

    const buildemployeeList = (data) => {
        $("#employeeList").empty();
        div = $(`<div class="list-group-item text-white bg-secondary row d-flex" id="status">Employee Info</div>
                  <div class="list-group-item row d-flex text-center" id="heading" >
                  <div class="col-4 h4">Title</div>
                  <div class="col-4 h4">First</div>
                  <div class="col-4 h4">Last</div>
                </div>`);
        div.appendTo($("#employeeList"));

        sessionStorage.setItem("allemployees", JSON.stringify(data));

        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add employee</button>`);
        btn.appendTo($("#employeeList"));

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

});//jQuery ready method

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