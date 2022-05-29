$(function () {

    $("#getbutton").click(async (e) => {
        $("#status").text("please wait...");

        try {
            //find employee via email
            let email = $("#TextBoxEmail").val();
            $("#status").text("please wait...");

            let response = await fetch(`/api/employee/${email}`);
            if (response.ok) {
                let data = await response.json();
                //check email
                if (data.email !== "not found") {
                    $('#email').text(data.email);
                    $("#title").text(data.title);
                    $("#firstname").text(data.firstname);
                    $("#lastname").text(data.lastname);
                    $("#phone").text(data.phoneno);
                    $("#status").text("Employee found");
                } else {
                    $("#firstname").text("not found");
                    $("#email").text("");
                    $("#title").text("");
                    $("#phone").text("");
                    $("#status").text("no such employee");
                }
            } else if (response.status !== 404) { //404 error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {//catastrophic failure
                $("#status").text(error.message);
            }

        } catch (error) {
            $("#status").text(error.message);
        }
    }); //click event
}); //jQuery ready method

//server was reached but there was a probelm with the call
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