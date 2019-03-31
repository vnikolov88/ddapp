var SlidesList = function () {
    var slides = [];
    var activeSlideIndex = 0;

    function addElement(element) {
        slides.push(element);
    }

    function next() {
        slides[activeSlideIndex].className = '';
        if (activeSlideIndex === (slides.length - 1)) {
            activeSlideIndex = -1;
        }
        slides[++activeSlideIndex].className = 'active';
    }

    function prev() {
        slides[activeSlideIndex].className = '';
        if (activeSlideIndex <= 0) {
            activeSlideIndex = slides.length;
        }
        slides[--activeSlideIndex].className = 'active';
    }

    function showByIndex(index) {
        slides[activeSlideIndex].className = '';
        activeSlideIndex = index;
        slides[activeSlideIndex].className = 'active';
    }

    function lastItemIndex() {
        return slides.length - 1;
    }

    function getActiveItemIndex() {
        return activeSlideIndex;
    }

    return {
        addElement: addElement,
        showPrev: prev,
        showNext: next,
        showOnPosition: showByIndex,
        lastItemIndex: lastItemIndex,
        getActiveItemIndex: getActiveItemIndex
    };
};

var simpleGallery = function (domElement) {
    var element = domElement;
    var slidesList = new SlidesList();
    var pagerList = [];
    var activePagerIndex = -1;

    var buttonPrev = element.getElementsByClassName("prev")[0].getElementsByTagName("button")[0];
    var buttonNext = element.getElementsByClassName("next")[0].getElementsByTagName("button")[0];

    buttonPrev.addEventListener('click', function () {
        slidesList.showPrev();
        updatePager();
    });
    buttonNext.addEventListener('click', function () {
        slidesList.showNext();
        updatePager();
    });

    function createPagerItemFor(index) {

        var pagerButton = document.createElement("button");
        pagerButton.addEventListener('click', function () {
            slidesList.showOnPosition(index);
            updatePager();
        });

        var pagerElement = document.createElement("li");
        pagerElement.appendChild(pagerButton);
        pagerList.push(pagerElement);

        var pagerListElem = element.getElementsByClassName("pager")[0].getElementsByTagName("ul")[0];
        pagerListElem.appendChild(pagerElement);
    }


    function updatePager() {
        if (activePagerIndex < 0) {
            activePagerIndex = 0;
            pagerList[activePagerIndex].className = 'active';
        }
        if (activePagerIndex !== slidesList.getActiveItemIndex()) {
            pagerList[activePagerIndex].className = '';
            pagerList[slidesList.getActiveItemIndex()].className = 'active';
            activePagerIndex = slidesList.getActiveItemIndex();
        }
    }


    function init() {
        var slides = element.getElementsByClassName("slides")[0].getElementsByTagName("li");
        for (var i = 0; i < slides.length; i++) {
            if ((slides[i].className).indexOf('skeleton') === -1) {
                slidesList.addElement(slides[i]);
                createPagerItemFor(slidesList.lastItemIndex());
            }
        }

        slidesList.showOnPosition(0);
        updatePager();
    }

    init();
};