window.stopClickPropagation = function (className) {
    var elements = document.getElementsByClassName(className);
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', function (event) {
            event.stopPropagation();
        });
    }
};