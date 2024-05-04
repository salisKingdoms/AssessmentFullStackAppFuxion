var table = null;
$(document).ready(function () {
    table = $('#listDataExp').DataTable({
        lengthMenu: [],
        lengthChange: false,
        searching: false
    });
});

function openExp() {
    $('#navProfile').removeClass('active');
    $('#navExp').addClass('active');
    $('#cvform').css('display', 'none');
    //$('#expform').css('display', 'block');
}

function openProfile() {
    $('#navProfile').addClass('active');
    $('#navExp').removeClass('active');
    $('#cvform').css('display', 'block');
    $('#expform').css('display', 'none');
}

function OnAddNewCV() {
    $('#listCV').css('display', 'none');
    $('#formCV').css('display', 'block');
}

function OnCloseCV() {
    $('#listCV').css('display', 'block');
    $('#formCV').css('display', 'none');

}

function addExperience() {
    $('#expform').css('display', 'block');
    $('#listExp').css('display', 'none');
}

function onAddNewExp(obj, id) {
    $('#listExp').css('display', 'block');
    //var table = $('#listDataExp').DataTable({
    //    lengthMenu: [],
    //    lengthChange: false,
    //    searching: false
    //});
    $('#listCV').css('display', 'none');
    $('#formCV').css('display', 'block');
    var totalRow = $("#listDataExp").find("tr:not(:first)").length;
    var dataExp = {
        indexRow: totalRow+1,
        empID: $('#cvID').val(),
        startYear: $('#start').val(),
        endYear: $('#end').val(),
        role: $('#role').val(),
        companyName: $('#companyName').val(),
        compAddress: $('#addressComp').val(),
        tools: $('#tools').val(),
        responsibility: $('#respDesc').val()
    };

    console.log(dataExp);
    var htmlRow = $("<tr>" + '<td style="display:none; text-align:center;">' + 0 + "</td>" +
        '<td style="text-align: center;">' + '<button class="btn btn-danger me-2"  type="submit"  onclick=OnDeletedExpNew(' + dataExp.indexRow + '); >Delete</button>' + "</td>" +
        '<td style="text-align:center;">' + dataExp.companyName + "</td>" +
        '<td style="text-align:center;">' + dataExp.role + "</td>" +
        '<td style="text-align:center;">' + dataExp.compAddress + "</td>" +
        '<td style="text-align:center;">' + dataExp.startYear + "</td>" +
        '<td style="text-align:center;">' + dataExp.endYear + "</td>" +
        '<td style="text-align:center;">' + dataExp.tools + "</td>" +
        '<td style="text-align:center;">' + dataExp.responsibility + "</td></tr>");

    table.row.add($(htmlRow)).draw();
    //clear form 
    $('#expform').css('display', 'none');
    $('#start').empty();
    $('#end').empty();
    //$('#role').empty();
    $('#companyName').empty();
    $('#addressComp').empty();
    $('#tools').empty();
    $('#respDesc').empty();

}

function OnDeletedExpNew(row) {
    var originalData = $('#listDataExp').DataTable().rows().data().toArray()
    $('#listDataExp tbody tr').eq(row).remove();
    var tables = $('#myDataTable').DataTable();
    tables.clear().rows.add(originalData).draw();
}


function saveData() {
    //if ($('#cvID').val() == "" || parseInt($('#cvID').val()) == 0) {
    //    var dataHeader = {
    //        name: $('#employeeName').val(),
    //        birth_date: $('#birthDate').val(),
    //        address: $('#address').val(),
    //        sallary: parseFloat($('#sallary').val()),
    //        nik: $('#employeeNIK').val(),
    //    };
    //    $.ajax({
    //        type: "POST",
    //        url: "/Employee/SubmitEmployee",
    //        data: dataHeader,
    //        success: function (respon) {
    //            if (respon.is_ok) {

    //                toastr.success("Employee success to submitted");
    //                setTimeout(() => {
    //                    $('#formCrud').css('display', 'none');
    //                    $('#formList').css('display', 'block');
    //                    searchDataListEmployee();
    //                }, 500);
    //            }
    //            else {
    //                console.log("Invoice failed to submitted");
    //                toastr.error(respon.messageUI);

    //            }
    //        }

    //    });
    //}
    //else {
    //    updateData();
    //}
}