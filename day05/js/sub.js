/* rotate-btn 동작 */
const rotateBtn = document.querySelector('.rotate-btn');
const slides = document.querySelectorAll('.bg-slide');
const totalSlides = slides.length; // 5
let index = 0;

// $('.rotate-btn').on('click', function()); 와 동일
rotateBtn.addEventListener('click', function(){
    rotateBtn.classList.add('active');
    setTimeout(() => {
        rotateBtn.classList.remove('active');
    }, 2100);

    slides.forEach(slide => {
        if (slide.classList.contains('active')) {
            slide.classList.add('after-active');
        } else {
            slide.classList.remove('after-active');
        }
    });

    slides[index].classList.remove('active'); // 현재 슬라이드 active는 제거
    index += 1;

    if (index == totalSlides) index = 0;

    slides[index].classList.add('active'); // 새 슬라이드에 active 추가
});