function openExp() {
    $('#navProfile').removeClass('active');
    $('#navExp').addClass('active');
    $('#cvform').css('display', 'none');
    $('#expform').css('display', 'block');
}

function openProfile() {
    $('#navProfile').addClass('active');
    $('#navExp').removeClass('active');
    $('#cvform').css('display', 'block');
    $('#expform').css('display', 'none');
}

function onAddNewCV() {
    $('#listCV').css('display', 'none');
    $('#formCV').css('display', 'block');
}

function OnCloseCV() {
    $('#listCV').css('display', 'block');
    $('#formCV').css('display', 'none');

}