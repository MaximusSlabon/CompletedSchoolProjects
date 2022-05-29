$(function () {

    const getAll = async (msg) => {
        try {
            $("#callList").text("Finding employee Information...");
            let response = await fetch(`api/call`);
            if (response.ok) {
                let payload = await response.json(); //this will return a promise so we await it 
                buildcallList(payload);
                msg === "" ? //are we appending to an existing message
                    $("#status").text("calls Loaded") : $("#status").text(`${msg} - calls Loaded`);
            } else if (response.status !== 404) { //probably some other client side error
                let problemJSON = await response.json();
                errorRtn(problemJSON, response.status);
            } else {//else 404 not found
                $("#status").text("no such path on server");
            }//else

//BUILD EMPLOYEE DROP DOWNS
            response = await fetch(`api/employee`);

            if (response.ok) {
                let payload = await response.json(); //this will return a promise so we await it 

                sessionStorage.setItem("allEmployees", JSON.stringify(payload));

                buildEmployeeDDL(payload);
                buildTechDDL(payload);

            } else if (response.status !== 404) { //probably some other client side error
                let problemJSON = await response.json();
                errorRtn(problemJSON, response.status);
            } else {//else 404 not found
                $("#status").text("no such path on server");
            }//else

//BUILD PROBLEM DROP DOWN
            response = await fetch(`api/problem`);

            if (response.ok) {
                let payload = await response.json(); //this will return a promise so we await it 

                buildProblemDDL(payload);

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
        $("#modaltitle").html("<h>update call</h4>");

        clearModalFields(); //clear everything

        data.map(call => {
            if (call.id === parseInt(id)) {
                $("#ddlProblems").val(call.problemId);
                $("#ddlEmployees").val(call.employeeId);
                $("#ddlTechs").val(call.techId);
                $("#TextBoxNotes").val(call.notes);
                sessionStorage.setItem("id", call.id);
                sessionStorage.setItem("timer", call.timer);
                //format date
                $("#dateOpened").text(formatDate(call.dateOpened).replace("T", " "));
                sessionStorage.setItem("dateOpened", formatDate(call.dateOpened));

                $("#deletebutton").show();
                $("#modalstatus").text("update data");
                $("#myModal").modal("toggle");
                $("#myModalLabel").text("Update");

                //check status of call
                if (!call.openStatus) {
                    //call is closed ->
                    //make everything readonly

                    $("#actionbutton").hide(); //hide update button

                    //disable elements
                    $("#ddlProblems").attr('disabled', true);
                    $("#ddlEmployees").attr('disabled', true);
                    $("#ddlTech").attr('disabled', true);

                    $("#checkboxStatus").attr('disabled', true);
                    $("#checkboxStatus").prop('checked', true); //"check" the check box
                    $("#TextBoxNotes").attr('readonly', true);

                    //set the closed date
                    $("#dateClosed").text(formatDate(call.dateclosed).replace("T", " "));
                    sessionStorage.setItem("dateClosed", formatDate(call.dateClosed));

                    $("#modalstatus").text("View or Delete Data");
                } else {
                    $("#checkboxStatus").prop('checked', false);
                    $("#dateClosed").text("");

                    //enable elements
                    $("#ddlProblems").attr('disabled', false);
                    $("#ddlEmployees").attr('disabled', false);
                    $("#ddlTech").attr('disabled', false);

                    $("#checkboxStatus").attr('disabled', false);
                    $("#checkboxStatus").prop('checked', false);
                    $("#TextBoxNotes").attr('readonly', false);
                }
            }//if
        }); //data.map
    };//setup for update

    const setupForAdd = () => {
        clearModalFields();
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add new call</h4>");

        $("#checkboxStatus").prop('checked', false);
        $("#dateClosed").text("");

        //enable elements
        $("#ddlProblems").attr('disabled', false);
        $("#ddlEmployees").attr('disabled', false);
        $("#ddlTech").attr('disabled', false);

        $("#checkboxStatus").attr('disabled', false);
        $("#checkboxStatus").prop('checked', false);
        $("#TextBoxNotes").attr('readonly', false);

        //set date opened to current time
        $("#dateOpened").text(formatDate().replace("T", " "));
        sessionStorage.setItem("dateOpened", formatDate());

        $("#checkboxStatusDIV").hide();
        $("#dateClosedDIV").hide();

        $("#myModal").modal("toggle");
        $("#modalstatus").text("enter data");
        $("#myModalLabel").text("Add");

        $("#deletebutton").hide();
    }; //setup for add

    const clearModalFields = () => {
        $("#ddlProblems").val(0);
        $("#ddlEmployees").val(0);
        $("#ddlTechs").val(0);
        $("#dateOpened").text("");
        $("#dateClosed").text("");
        $("#textBoxNotes").val("");
        
        $("#actionbutton").show();
        $("myModal").modal("toggle");
        $("#CallModalForm").validate().resetForm();
    }; //clearModalFields

    const add = async () => {
        try {
            //set up a new client side instance of employee
            stu = new Object();
            //pupulate
            stu.problemId = $("#ddlProblems").val();
            stu.employeeId = $("#ddlEmployees").val();
            stu.techId = $("#ddlTech").val();

            stu.dateOpened = sessionStorage.getItem("dateOpened");
            stu.dateClosed = null;
            stu.openStatus = true;

            stu.notes = $("#TextBoxNotes").val();
            stu.timer = null;

            //send the updated back to the server async using POST
            let response = await fetch("api/call", {
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
            stu.problemId = $("#ddlProblems").val();
            stu.employeeId = $("#ddlEmployees").val();
            stu.techId = $("#ddlTech").val();

            stu.dateOpened = sessionStorage.getItem("dateOpened");
            stu.dateClosed = sessionStorage.getItem("dateClosed");

            stu.problemDescription = $("#ddlProblems").text();
            stu.employeeName = $("#ddlEmployees").text();

            stu.techName = $("#ddlTech").text();

            if ($("#checkboxStatus").is(":checked")) {
                stu.openStatus = false; // I dont know how to deal with this
            } else {
                stu.openStatus = true;
            }

            stu.notes = $("#TextBoxNotes").val();

            stu.id = sessionStorage.getItem("id");
            stu.timer = sessionStorage.getItem("timer");
            
            //send the updated back to the server asynch using PUT
            let response = await fetch("api/call", {
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

    const _delete = async () => {
        try {

            let response = await fetch(`api/call/${sessionStorage.getItem("id")}`, {
                method: 'DELETE',
                headers: { "Content-Type": "application/json; charset=utf-8" }
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

            $("#myModal").modal("toggle");
        } catch (error) {
            $("#status").text(error.message);
        } //try/catch
    };//delete

    $("#actionbutton").click(() => {
        //button logic for generating right stuff
        $("#actionbutton").val() === "update" ? update() : add();
    });

    $("#callList").click((e) => {
        clearModalFields();
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "callList" || id === "") {
            id = e.target.id;
        }//clicked elsewhere

        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allCalls"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false; // ignore if the clicked on heading or status
        }
    })//callListClick

    $("#deletebutton").click(() => {
        if (window.confirm("Are you sure?")) {
            _delete();
        }
    });

    $("#srch").keyup(() => {
        let alldata = JSON.parse(sessionStorage.getItem("allCalls"));
        let filtereddata = alldata.filter((stu) => stu.employeeName.match(new RegExp($("#srch").val(), 'i')));
        buildcallList(filtereddata, false);
    });//srch keyup

    $("#checkboxStatus").click(() => {
        if ($("#checkboxStatus").is(":checked")) {
            $("#dateClosed").text(formatDate().replace("T", " "));
            sessionStorage.setItem("dateClosed", formatDate());
        } else {
            $("#dateClosed").text("");
            sessionStorage.setItem("dateClosed", "");
        }
    }); //checkbox click

    const buildEmployeeDDL = (data) => {
        html = "";
        $("#ddlEmployees").empty();

        data.map(employee => html += `<option value="${employee.id}">${employee.lastname}</option>`);
        $("#ddlEmployees").append(html);
    }

    const buildTechDDL = (data) => {
        html = "";
        $("#ddlTech").empty();

        const techs = [];

        for (let i = 0; i < data.length; i++) {
            if (data[i].isTech == true) {
                techs.push(data[i]);
            }
        }

        techs.map(employee => html += `<option value="${employee.id}">${employee.lastname}</option>`);
        $("#ddlTech").append(html);
    }

    const buildProblemDDL = (data) => {
        html = "";
        $("#ddlProblems").empty();

        data.map(problem => html += `<option value="${problem.id}">${problem.description}</option>`);
        $("#ddlProblems").append(html);
    }

    const buildcallList = (data, usealldata = true) => {
        $("#callList").empty();
        div = $(`<div class="list-group-item text-white bg-secondary row d-flex" id="status">Call Info</div>
                  <div class="list-group-item row d-flex text-center" id="heading" >
                  <div class="col-4 h4">Date</div>
                  <div class="col-4 h4">For</div>
                  <div class="col-4 h4">Problem</div>
                </div>`);
        div.appendTo($("#callList"));

        usealldata ? sessionStorage.setItem("allCalls", JSON.stringify(data)) : null;

        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add call</button>`);
        btn.appendTo($("#callList"));

        data.map(stu => {
            btn = $(`<button class="list-group-item row d-flex" id="${stu.id}">`);
            btn.html(
                `<div class="col-4" id="calldate${stu.id}">${formatDate(stu.dateOpened).replace("T", " ")}</div>
                <div class="col-4" id="employeename${stu.id}">${stu.employeeName}</div>
                <div class="col-4" id="problem${stu.id}">${stu.problemDescription}</div>`
            );
            btn.appendTo($("#callList"));
        });//map
    }//buildcallList

    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) { _day = "0" + _day; }
        let _month = d.getMonth() + 1;
        if (_month < 10) { _month = "0" + _month; }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) { _hour = "0" + _hour; }
        let _min = d.getMinutes();
        if (_min < 10) { _min = "0" + _min; }

        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } //format date

    document.addEventListener("input", e => {
        $("#modalstatus").removeClass();
        if ($("#CallModalForm").valid()) {
            $("#modalstatus").attr("class", "badge badge-valid");
            $("#modalstatus").text("data entered is valid");
            $("#actionbutton").prop('disabled', false);
        }
        else {
            $("#modalstatus").attr("class", "badge badge-invalid");
            $("#modalstatus").text("fix errors");
            $("#actionbutton").prop('disabled', true);
        }
    });

    $("#CallModalForm").validate({
        rules: {
            ddlProblems: { required: true },
            ddlEmployees: { required: true },
            ddlTech: { required: true },
            TextBoxNotes: { rangelength: [1, 250], required: true }
        },
        errorElement: 'div',
        messages: {
            ddlProblems: { required: "Select Problem" },
            ddlEmployees: { required: "Select Employee" },
            ddlTech: { required: "Select Technician" },
            TextBoxNotes: { rangelength: "Required 1-250 Characters", required: "Required 1-250 Characters" }
        }
    });//CallModalForm.validate

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