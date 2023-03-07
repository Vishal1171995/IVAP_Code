
$(document).ready(function () {
    document.querySelector(".hamburger").onclick = function () {
        var element = document.querySelector(".leftMenu");
        element.classList.toggle("openMenu");

        var hamburger = document.querySelector(".hamburger");
        hamburger.classList.toggle("open");

        var closeAccordion = document.getElementsByClassName('dropdown');
        var i = 0;
        for (i = 0; i < closeAccordion.length; i++) {
            closeAccordion[i].classList.remove('active');
        }
    }

    var dropdown = document.getElementsByClassName('dropdown');
    var i = 0;
    for (i = 0; i < dropdown.length; i++) {
        dropdown[i].onclick = function () {
            this.classList.toggle('active');
        }
    }
});

$(".hamburger").click(function () {
    $(".ContentRightMain").toggleClass("MenuOpenContentRight");
    $(".leftMenu").toggleClass("leftMenuOpen");

});

document.onmouseover = function (e) {
    var targ;
    if (!e) var e = window.event;
    if (e.target) targ = e.target;
    else if (e.srcElement) targ = e.srcElement;
    if (targ.nodeType == 3) // defeat Safari bug
        targ = targ.parentNode;

    if (targ.id == 'leftMenu' && $('.leftMenu').hasClass('leftMenuOpen') != true) {

        

    } else {
        if ($('.leftMenu').width() < 100) {
        var closeAccordion = document.getElementsByClassName('dropdown');
        var i = 0;
        for (i = 0; i < closeAccordion.length; i++) {
            closeAccordion[i].classList.remove('active');
        }
        }
    }
}

$(document).mousemove(function (event) {
    if ($('.leftMenu').width() < 100) {
        var closeAccordion = document.getElementsByClassName('dropdown');
        var i = 0;
        for (i = 0; i < closeAccordion.length; i++) {
            closeAccordion[i].classList.remove('active');
        }
    }
});

$(".leftMenu ").mouseleave(function (event) {
    
    if ($(".hamburger").hasClass('open') != true) {
        var closeAccordion = document.getElementsByClassName('dropdown');
        var i = 0;
        for (i = 0; i < closeAccordion.length; i++) {
            closeAccordion[i].classList.remove('active');
        }
    }
    
});