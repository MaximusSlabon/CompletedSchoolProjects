$(function () {
    $("#employeebutton").click(async (e) => {
        try {
            $("#lblstatus").text("Generating report on server - please wait...");
            let response = await fetch(`api/employeereport`);
            if (!response.ok)
                throw new Error(`Status - ${response.status}, Text ${response.statusText}`);
            let data = await response.json();
            data.msg === "Report Generated"
                ? window.open("/pdfs/employeelist.pdf")
                : $("#lblstatus").text("problem generating report");
        } catch (error) {
            $("#lblstatus").text(error.message);
        }
    });

    $("#callbutton").click(async (e) => {
        try {
            $("#lblstatus").text("Generating report on server - please wait...");
            let response = await fetch(`api/callreport`);
            if (!response.ok)
                throw new Error(`Status - ${response.status}, Text ${response.statusText}`);
            let data = await response.json();
            data.msg === "Report Generated"
                ? window.open("/pdfs/calllist.pdf")
                : $("#lblstatus").text("problem generating report");
        } catch (error) {
            $("#lblstatus").text(error.message);
        }
    });
})