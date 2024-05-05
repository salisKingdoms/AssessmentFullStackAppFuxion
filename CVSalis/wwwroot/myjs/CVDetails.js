var table = null;
var dtCV = null;
var tempExpNew = [];
var rowTempExpDelete = 0;
var isNego = false;
var pathImage = "";
$(document).ready(function () {
    dtCV = $('#listData').DataTable({
        lengthMenu: [],
        lengthChange: false,
        searching: false
    });
    OnLoadListCV();
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

function OnChangeNegotiable() {
    if ($('#isNego').val() == "0") {
        $('#isNego').val("1");
        isNego = true;
        $('#lbNego').text('True');
    }
    else {
        
        isNego = false;
        $('#lbNego').text('False');
        $('#isNego').val("0");
    }
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

function OnUploadImage() {
 
    var uploadfile = document.getElementById("upload");
    var files = uploadfile.files;
    var filedata = new FormData();
    for (var i = 0; i < files.length; i++) {
        filedata.append("atc", files[i]);
    }
  
    $.ajax({
        type: "POST",
        url: "/CV/DataUpload",
        data: filedata,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.message == "OK") {
                pathImage = response.filePath;
                $('#uploadedAvatar').attr('src', "/image/" + pathImage );
                console.log(pathImage);
            }
        }
    });
}

function OnSaveCV() {
    if ($('#cvID').val() == '') {
        $('#mySpinner').css('display', 'block');
        var mappingDetail = [];
        if (tempExpNew.length > 0) {
            $.each(tempExpNew, function (i, data) {
                var items = {
                    employee_id: 0,
                    company: data.companyName,
                    company_address: data.compAddress,
                    role: data.role,
                    periode_start: data.startYear,
                    periode_end: data.endYear,
                    resposibility_desc: data.responsibility,
                    tech_tools: data.tools
                };
                mappingDetail.push(items);
            });
        }

        var dataCV = {
            isCreated: true,
            employee_no: 0,
            employee_name: $('#fullName').val(),
            phone: $('#phoneNumber').val(),
            email: $('#email').val(),
            birth_date: $('#birthDate').val(),
            address: $('#address').val(),
            ktp: $('#ktp').val(),
            image: "/image/" + pathImage, 
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
            is_negotiable: isNego,
            is_deleted: false,
            Experience_List: mappingDetail
        };

        $.ajax({
            type: "POST",
            url: "/CV/SubmitNewCV",
            data: dataCV,
            success: function (respon) {
                if (respon.is_ok) {

                    toastr.success("CV success to submitted");
                    setTimeout(() => {
                        $('#mySpinner').css('display', 'none');
                        
                        OnLoadListCV();
                    }, 500);
                }
                else {
                    console.log("Invoice failed to submitted");
                    toastr.error(respon.messageUI);
                    $('#mySpinner').css('display', 'none');
                }
            }
        });

    }
   
}

function OnLoadListCV() {
    $('#mySpinner').css('display', 'block');
    var dataHeader = { };

    $.ajax({
        type: "GET",
        url: "/CV/GetListCV",
        data: dataHeader,
        processData: false,
        contentType: false,
        success: function (respon) {
            var dataresp = JSON.parse(respon);
            if (dataresp.is_ok) {
                if (dataresp.listCV.length > 0) {

                    $.each(dataresp.listCV, function (i, data) {//Name,pos,gender,phone,email,total
                        var genders = (data.gender == 2 ? "Female" : "Male");
                        var htmlRow = $("<tr>" + '<td style="display:none; text-align:center;">' + data.employee_no + "</td>" +
                            '<td  style="text-align: center;">' + '<button id=' + data.employee_no + ' class="btn btn-primary me-2"  type="submit"  onclick=OnPDFCV(this);  >PDF</a>' + "</td>" +
                            '<td  style="text-align: center;">' + '<button id=' + data.employee_no + ' class="btn btn-primary me-2"  type="submit"  onclick=OnEditCV(this); >Edit</button>' + "</td>" +
                            '<td  style="text-align: center;">' + '<button id=' + data.employee_no + ' class="btn btn-danger me-2"  type="submit"  onclick=OnDeletedCV(this); >Delete</button>' + "</td>" +
                            '<td style="text-align:center;">' + data.employee_name + "</td>" +
                            '<td style="text-align:center;">' + data.position + "</td>" +
                            '<td style="text-align:center;">' + genders + "</td>" +
                            '<td style="text-align:center;">' + data.phone + "</td>" +
                            '<td style="text-align:center;">' + data.email + "</td>" +
                            '<td style="text-align:center;">' + data.total_exp + "</td></tr>");
                        dtCV.row.add($(htmlRow)).draw();
                    });
                    //var row = dtCV.row(0).node();
                    //$(row).find('td').eq(0).attr('colspan', 2);
                   // $('#listData thead th:eq(0)').next().remove();
                    $('#listCV').css('display', 'block');
                    $('#formCV').css('display', 'none');
                    $('#mySpinner').css('display', 'none');

                }
                else {
                    $('#listCV').css('display', 'block');
                    $('#formCV').css('display', 'none');
                    console.log("employee not found");
                    $('#mySpinner').css('display', 'none');
                }
            }
        }
    });
}

function OnEditCV(obj) {
    var cvIdChoosen = parseInt($(obj).attr('id'));
     var dataHeader = {
         id: cvIdChoosen 
    };
    $('#mySpinner').css('display', 'block');
    $.ajax({
        type: "GET",
        url: "/CV/GetDetailCVById",
        data: dataHeader,
        success: function (respon) {
            var dataresp = JSON.parse(respon);
            if (dataresp.is_ok) {
                $('#uploadedAvatar').attr('src', dataresp.dataDetail.image);
                $('#cvID').val(dataresp.dataDetail.employee_no);
                $('#fullName').val(dataresp.dataDetail.employee_name);
                $('#phoneNumber').val(dataresp.dataDetail.phone);
                $('#email').val(dataresp.dataDetail.email);
                //$('#birthDate').val(dataresp.dataDetail.birth_date);
                $('#address').val(dataresp.dataDetail.address);
                $('#ktp').val(dataresp.dataDetail.ktp);
                $('#softSkill').val(dataresp.dataDetail.soft_skill);
                $('#hardSkill').val(dataresp.dataDetail.hard_skill);
                $('#gender').val(dataresp.dataDetail.gender);
                $('#maritalID').val(dataresp.dataDetail.marital_status);
                $('#expSallary').val(dataresp.dataDetail.expectation_sallary);
                $('#eduType').val(dataresp.dataDetail.education_type);
                $('#eduName').val(dataresp.dataDetail.education_name);
                $('#eduIPK').val(dataresp.dataDetail.ipk);
                $('#eduYear').val(dataresp.dataDetail.year_education);
                $('#npwp').val(dataresp.dataDetail.npwp);
                $('#position').val(dataresp.dataDetail.position);
                $('#countExp').val(dataresp.dataDetail.total_exp);
                $('#isNego').val(dataresp.dataDetail.is_negotiable);
                isNego = dataresp.dataDetail.is_negotiable;
                if (isNego == true) {
                    $('#lbNego').text('True');
                } else {
                    $('#lbNego').text('False');
                }
                
                $('#listCV').css('display', 'none');
                $('#formCV').css('display', 'block');
                $('#listExp').css('display', 'none');
                //hide spinner 3 seconds
                setTimeout(function () {
                    $('#mySpinner').css('display', 'none');
                }, 3000);
            }
            else {
                console.log("employee not found");
                //hide spinner 3 seconds
                setTimeout(function () {
                    $('#mySpinner').css('display', 'none');
                }, 3000);
            }
        }

    });
}

function OnDeletedCV(obj) {

}

function OnPDFCV(obj) {
    var cvIdChoosen = parseInt($(obj).attr('id'));
    $('#mySpinner').css('display', 'block');
    var paramData = {
        cvID: cvIdChoosen
    };
    $.ajax({
        type: "POST",
        url: "/CV/GeneratePDF",
        data: paramData,
        success: function (response) {
            if (!response.is_ok) {
                toastr.error(response.message);
                $('#mySpinner').css('display', 'none');
            }
            else {
    
                var str = response.data.filePath;
                window.open(str, '_blank');
                
                $('#mySpinner').css('display', 'none');
            }
        }
    });
}