function init() {
    var nuggetTitles = document.querySelectorAll('.NuggetTitle'),
        nuggetContainers = document.querySelectorAll(".Nugget"),
        pillTitles = document.querySelectorAll('.nav-pills > .nav-item'), // the target is suppose to be the .nav-item instead of the .nav-item.
        pillContainers = document.querySelectorAll('.tab-pane');


    var collections = [nuggetTitles, nuggetContainers, pillTitles, pillContainers];


    function ActivateFirstElement(collection) {
        collection[0].classList.add('Active');
    }

    function Activate(sequence, target) {
        for(let i = 0; i < sequence.length; i++) {
            if(sequence[i].classList.contains('Active')) {
               sequence[i].classList.remove('Active')
            }
        }

        //Commented out because the target is the link but we want to highligh the parentElement because it needs to carry the background with the .Active classname.
        //sequence.forEach(function (element) {
        //    if (element.classList.contains('Active'))
        //        element.classList.remove('Active');
        //});

        target.classList.add('Active');
    }

    function _ifCollectionExists(collection) {
        if (collection.length === 0) {
            return false;
        } else {
            return true;
        }
    }

    collections.forEach(function (collection) {
        if (_ifCollectionExists(collection)) {
            ActivateFirstElement(collection);
        }
    });

    //hook Pills
    if (_ifCollectionExists(pillTitles)) {
        pillTitles.forEach(function (pillTitle) {
            pillTitle.addEventListener('click', function (evt) {
                var containerTarget = document.getElementById(evt.target.id.slice(0, -4)),
                    titleTarget = evt.target;

                Activate(pillContainers, containerTarget);
                //changed because the target is the link but we want to highligh the parentElement because it needs to carry the background with the .Active classname.
                Activate(pillTitles, titleTarget.parentElement);

                evt.stopPropagation();

            });
        });
    }

    //hook Nuggets
    if (_ifCollectionExists(nuggetTitles)) {
        nuggetTitles.forEach(function (title) {
            title.addEventListener('click', function (evt) {

                var containerTarget = document.getElementById(evt.target.id.slice(0, -4)),
                    titleTarget = evt.target;

                Activate(nuggetContainers, containerTarget);
                Activate(nuggetTitles, titleTarget);

                evt.stopPropagation();
            });
        });
    }
}


export default {
    init
};