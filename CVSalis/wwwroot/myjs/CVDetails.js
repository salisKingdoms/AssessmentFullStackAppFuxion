var table = null;
var dtCV = null;
var tempExpNew = [];
var rowTempExpDelete = 0;
$(document).ready(function () {
    dtCV = $('#listData').DataTable({
        lengthMenu: [],
        lengthChange: false,
        searching: false
    });

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
    $('#listExp').css('display', 'block');
}

function openProfile() {
    $('#navProfile').addClass('active');
    $('#navExp').removeClass('active');
    $('#cvform').css('display', 'block');
    $('#expform').css('display', 'none');
    $('#listExp').css('display', 'none');
}

function OnAddNewCV() {
    $('#listCV').css('display', 'none');
    $('#formCV').css('display', 'block');
    $('#listExp').css('display', 'none');
}

function OnCloseCV() {
    $('#listCV').css('display', 'block');
    $('#formCV').css('display', 'none');

}

function addExperience() {
    $('#expform').css('display', 'block');
    $('#listExp').css('display', 'none');
}

function onCloseExpForm() {
    $('#expform').css('display', 'none');
    $('#listExp').css('display', 'block');
}

function OnCloseAlert() {
    $('#modalExpAlert').css('display', 'none');
    $('#modalExpAlert').modal("hide");
}

function startKeyUp() {
    if ($('#start').val().length > 4)
        $('#start').val(($('#start').val()).slice(0, 4));
}

function endKeyUp() {
    if ($('#end').val().length > 4)
        $('#end').val(($('#end').val()).slice(0, 4));
}

function onAddNewExp(obj, id) {
    if ($('#start').val() == '' && $('#end').val() == ''  && $('#role').val() == '' && $('#companyName').val() == '' && $('#addressComp').val() == '' && $('#tools').val() == '' && $('#respDesc').val() == '') {
        $('#modalExpAlert').css('display', 'block');
        $('#modalExpAlert').modal("show");
    }
    else {
        $('#listExp').css('display', 'block');

        $('#listCV').css('display', 'none');
        $('#formCV').css('display', 'block');
        var totalRow = table.rows().count();
        var dataExp = {
            indexRow: totalRow + 1,
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
        tempExpNew.push(dataExp);
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
        $('#start').val('');
        $('#end').val('');
        $('#role').val('');
        $('#companyName').val('');
        $('#addressComp').val('');
        $('#tools').val('');
        $('#respDesc').val('');
    }
    

}

function OnDeletedExpNew(row) {
    rowTempExpDelete = row;
    $('#modalExpTemp').css('display', 'block');
    $('#modalExpTemp').modal("show");
    
}

function OnDeleteOK() {
    
    $('#modalExpTemp').modal("hide");
    $('#mySpinner').css('display', 'block');

    tempExpNew = tempExpNew.splice(rowTempExpDelete, 1);
    console.log("ini data temp:" + tempExpNew);
    $.each(tempExpNew, function (i, item) {
        item.indexRow -= 1;
    });

    rowTempExpDelete = 0;
    table.clear();
    $("#listDataExp").find("tr:not(:first)").remove();
    $('#listDataExp').DataTable().destroy();

    $.each(tempExpNew, function (i, data) {
        var htmlRow = $("<tr>" + '<td style="display:none; text-align:center;">' + 0 + "</td>" +
            '<td style="text-align: center;">' + '<button class="btn btn-danger me-2"  type="submit"  onclick=OnDeletedExpNew(' + data.indexRow + '); >Delete</button>' + "</td>" +
            '<td style="text-align:center;">' + data.companyName + "</td>" +
            '<td style="text-align:center;">' + data.role + "</td>" +
            '<td style="text-align:center;">' + data.compAddress + "</td>" +
            '<td style="text-align:center;">' + data.startYear + "</td>" +
            '<td style="text-align:center;">' + data.endYear + "</td>" +
            '<td style="text-align:center;">' + data.tools + "</td>" +
            '<td style="text-align:center;">' + data.responsibility + "</td></tr>");

        table.row.add($(htmlRow));

    });

    //redraw datatable with new array after splice/deleted
    table.draw();
    //hide spinner 3 seconds
    setTimeout(function () {
        $('#mySpinner').css('display', 'none');
    }, 3000);
}

function OnCloseModal() {
    $('#modalExpTemp').css('display', 'none');
    $('#modalExpTemp').modal("hide");
}

function OnSaveCV() {
    if ($('#cvID').val() == '') {
        var dataCV = {
            isCreated: true,
            employee_no: 0,
            employee_name: $('#fullName').val(),
            phone: $('#phoneNumber').val(),
            email: $('#email').val(),
            birth_date: $('#birthDate').val(),
            address: $('#address').val(),
            ktp: $('#ktp').val(),
            image: "",
            soft_skill: $('#softSkill').val(),
            hard_skill: $('#hardSkill').val(),
            gender: $('#gender').val(),
            marital_status: $('#maritalID').val(),
            expectation_sallary: $('#expSallary').val(),
            education_type: $('#eduType').val(),
            education_name: $('#eduName').val(),
            ipk: $('#eduIPK').val(),
            year_education: $('#eduYear').val(),
            total_exp: $('#expSallary').val(),
            npwp: $('#npwp').val(),
            position: $('#position').val(),
            focus_education: $('#eduType').val(),
            is_negotiable: $('#isNego').val(),
            is_deleted: false,
            Experience_List: tempExpNew
        };

        $.ajax({
            type: "POST",
            url: "/CV/SubmitNewCV",
            data: dataCV,
            success: function (respon) {
                if (respon.is_ok) {

                    toastr.success("CV success to submitted");
                    setTimeout(() => {
                        $('#listCV').css('display', 'block');
                        $('#formCV').css('display', 'none');
                       // searchDataListEmployee();
                    }, 500);
                }
                else {
                    console.log("Invoice failed to submitted");
                    toastr.error(respon.messageUI);

                }
            }
        });

    }
   
}