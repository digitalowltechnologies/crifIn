// calculate header height
var header = document.getElementById("navbar");
var headerHeight = header.clientHeight;
var root = document.querySelector(":root");
root.style.setProperty("--headerHeight", headerHeight + "px");

/*  ---------- ON-SCROLL ANIMATION ------------  */
var $animation_elements = $(".animateThis");
var $window = $(window);

function check_if_in_view() {
	var window_height = $window.height();
	var window_top_position = $window.scrollTop();
	var window_bottom_position = window_top_position + window_height - 80;

	$.each($animation_elements, function () {
		var $element = $(this);
		var element_height = $element.outerHeight();
		var element_top_position = $element.offset().top;
		var element_bottom_position = element_top_position + element_height;

		//check to see if this current container is within viewport ----
		// if ((element_top_position <= window_bottom_position) && (element_bottom_position >= window_top_position))  {

		if (element_top_position <= window_bottom_position) {
			$element.addClass("in-view");
		} else {
			$element.removeClass("in-view");
		}
	});
}

$window.on("scroll resize", check_if_in_view);
$window.trigger("scroll");

!(function (a) {
	a.fn.slickAnimation = function () {
		function n(a, n, t, i, o) {
			(o = void 0 !== o && o), 1 == n.opacity ? (a.addClass(t), a.addClass(i)) : (a.removeClass(t), a.removeClass(i)), o && a.css(n);
		}
		function t(a, n) {
			return a ? 1e3 * a + 1e3 : n ? 1e3 * n : a || n ? 1e3 * a + 1e3 * n : 1e3;
		}
		function i(a, n, t) {
			var i = {};
			["animation-" + n, "-webkit-animation-" + n, "-moz-animation-" + n, "-o-animation-" + n, "-ms-animation-" + n].forEach(function (a) {
				i[a] = t + "s";
			}),
				a.css(i);
		}
		var o = a(this),
			e = o.find(".slick-list .slick-track > div"),
			s = o.find('[data-slick-index="0"]'),
			r = "animated",
			c = { opacity: "1" },
			d = { opacity: "0" };
		return (
			e.each(function () {
				var e = a(this);
				e.find("[data-animation-in]").each(function () {
					var u = a(this);
					u.css(d);
					var l = u.attr("data-animation-in"),
						f = u.attr("data-animation-out"),
						h = u.attr("data-delay-in"),
						m = u.attr("data-duration-in"),
						y = u.attr("data-delay-out"),
						C = u.attr("data-duration-out");
					f
						? (s.length > 0 &&
								e.hasClass("slick-current") &&
								(n(u, c, l, r, !0),
								h && i(u, "delay", h),
								m && i(u, "duration", m),
								setTimeout(function () {
									n(u, d, l, r), n(u, c, f, r), y && i(u, "delay", y), C && i(u, "duration", C);
								}, t(h, m))),
						  o.on("afterChange", function (a, o, s) {
								e.hasClass("slick-current") &&
									(n(u, c, l, r, !0),
									h && i(u, "delay", h),
									m && i(u, "duration", m),
									setTimeout(function () {
										n(u, d, l, r), n(u, c, f, r), y && i(u, "delay", y), C && i(u, "duration", C);
									}, t(h, m)));
						  }),
						  o.on("beforeChange", function (a, t, i) {
								n(u, d, f, r, !0);
						  }))
						: (s.length > 0 && e.hasClass("slick-current") && (n(u, c, l, r, !0), h && i(u, "delay", h), m && i(u, "duration", m)),
						  o.on("afterChange", function (a, t, o) {
								e.hasClass("slick-current") && (n(u, c, l, r, !0), h && i(u, "delay", h), m && i(u, "duration", m));
						  }),
						  o.on("beforeChange", function (a, t, i) {
								n(u, d, l, r, !0);
						  }));
				});
			}),
			this
		);
	};
})(jQuery);

// sticky header
$(window).scroll(function () {
	if ($(window).scrollTop() > 0) {
		$("header").addClass("sticky");
		// $('.topNav').slideUp();
	} else {
		$("header").removeClass("sticky");
		// $('.topNav').slideDown();
	}
});

// mobile menu btn
$(".menuBtn").click(function () {
	$(this).toggleClass("active");
	$(".menuWrp").toggleClass("active");
	$("body").toggleClass("disableScroll");
});

$(".hasDropdown").on("click", function (event) {
	// $('.navList .hasDropdown').removeClass('active');
	$(this).siblings(".hasDropdown").find(".dropDown").removeClass("active");
	if (!$(this).find(".dropDown").hasClass("active") && window.innerWidth <= 991) {
		event.preventDefault();
		$(this).find(".dropDown").addClass("active");
		$(this).find(".dropDown").focus();
		return false;
	} else {
		$(this).find(".dropDown").removeClass("active");
	}
});

$(".dropDown").on("blur", function () {
	$(this).hide();
});

//  create triangle cut-out image in image boxes
$(function () {
	$(".iconCardImg").each(function () {
		$(this)
			.find(".imgWrapper")
			.css("--rotate", "rotate(" + $(this).outerHeight() / 10 + "deg)");
	});
});

//  toggle search bar
$(".search-toggler").click(function () {
	$(".search-toggler").toggleClass("open");
	$(".menuWrp").removeClass("active");
	$(".search-ovl-wrp").toggleClass("open");
	// $('body').toggleClass('disableScroll')
});

// copywrite year
$(function () {
	var currentDate = new Date();
	$("#footerYear").html(currentDate.getFullYear());
});

// custom slider
var customSwiper = new Swiper(".customSwiper", {
	lazy: true,
	loop: true,
	autoplay: {
		delay: 1000,
		disableOnInteraction: false,
	},
	speed: 1000,
	slidesPerView: 4,
	// spaceBetween: 15,
	// initialSlide: 1,
	pagination: {
		el: ".swiper-pagination",
		clickable: true,
	},
	breakpoints: {
		500: {
			slidesPerView: 1,
		},
		992: {
			slidesPerView: 2,
		},
		1200: {
			slidesPerView: 3,
		},
	},
});

// custom slider
var blogsSwiper = new Swiper(".blogsSwiper", {
	lazy: true,
	loop: true,
	autoplay: {
		delay: 1000,
		disableOnInteraction: false,
	},
	speed: 1500,
	slidesPerView: 1,
	pagination: {
		el: ".swiper-pagination",
		clickable: true,
	},
});

// centered slider
var centerSwiper = new Swiper(".centeredSwiper", {
	lazy: true,
	loop: true,
	autoplay: {
		delay: 1000,
		disableOnInteraction: false,
	},
	speed: 1000,
	slidesPerView: 3,
	spaceBetween: 10,
	centeredSlides: true,
	initialSlide: 1,
	pagination: {
		el: ".swiper-pagination",
		clickable: true,
	},
	breakpoints: {
		500: {
			slidesPerView: 1,
		},
		1024: {
			slidesPerView: 2,
			centeredSlides: false,
			spaceBetween: 20,
		},
	},
});

// HERO SLIDER
var menu = [];
jQuery(".hero-slider .swiper-slide").each(function (index) {
	menu.push(jQuery(this).find(".slide-inner").attr("data-text"));
});
var interleaveOffset = 0.5;
var swiperOptions = {
	loop: true,
	lazy: true,
	speed: 1500,
	parallax: true,
	autoplay: {
		delay: 3000,
		disableOnInteraction: false,
	},
	watchSlidesProgress: true,
	pagination: {
		el: ".hero-slider .swiper-pagination",
		clickable: true,
	},
	// navigation: {
	//     nextEl: '.swiper-button-next',
	//     prevEl: '.swiper-button-prev',
	// },
	on: {
		progress: function () {
			var swiper = this;
			for (var i = 0; i < swiper.slides.length; i++) {
				var slideProgress = swiper.slides[i].progress;
				var innerOffset = swiper.width * interleaveOffset;
				var innerTranslate = slideProgress * innerOffset;
				swiper.slides[i].querySelector(".slide-bg-image").style.transform = "translate3d(" + innerTranslate + "px, 0, 0)";
			}
		},
		touchStart: function () {
			var swiper = this;
			for (var i = 0; i < swiper.slides.length; i++) {
				swiper.slides[i].style.transition = "";
			}
		},
		setTransition: function (speed) {
			var swiper = this;
			for (var i = 0; i < swiper.slides.length; i++) {
				swiper.slides[i].style.transition = speed + "ms";
				swiper.slides[i].querySelector(".slide-bg-image").style.transition = speed + "ms";
			}
		},
	},
};

var swiper = new Swiper(".hero-slider .swiper-container", swiperOptions);

// DATA BACKGROUND IMAGE
var sliderBgSetting = $(".slide-bg-image");
sliderBgSetting.each(function (indx) {
	if ($(this).attr("data-background")) {
		$(this).css("background-image", "url(" + $(this).data("background") + ")");
	}
});

// Counter
var a = 0;
$(window).scroll(function () {
	var counters = $(".counters");
	var oTop = counters.length > 0 ? counters.offset().top - window.innerHeight : 0;

	if (a == 0 && $(window).scrollTop() > oTop) {
		$(".counterNum").each(function () {
			var $this = $(this);
			jQuery({ Counter: 0 }).animate(
				{ Counter: $this.text().replace("+", "") },
				{
					duration: 2000,
					easing: "swing",
					step: function () {
						$this.text(Math.ceil(this.Counter) + "+");
					},
				}
			);
		});
		a = 1;
	}
});

// parallax
$(window).scroll(function () {
	$(".innerBanner").css("background-position", "50% " + $(this).scrollTop() / 5 + "%");
});

// verticle tabs
$(".tab_content").hide();
$(".tab_content:first").show();

/* if in tab mode */
$("ul.tabs li").click(function () {
	$(".tab_content").hide();
	var activeTab = $(this).attr("rel");
	$("#" + activeTab).fadeIn();

	$("ul.tabs li").removeClass("active");
	$(this).addClass("active");

	$(".tab_drawer_heading").removeClass("d_active");
	$(".tab_drawer_heading[rel^='" + activeTab + "']").addClass("d_active");

	/*$(".tabs").css("margin-top", function(){ 
      return ($(".tab_container").outerHeight() - $(".tabs").outerHeight() ) / 2;
   });*/
});

$(".tab_container").css("min-height", function () {
	return $(".tabs").outerHeight() + 50;
});
/* if in drawer mode */
$(".tab_drawer_heading").click(function () {
	$(".tab_content").hide();
	var d_activeTab = $(this).attr("rel");
	$("#" + d_activeTab).fadeIn();

	$(".tab_drawer_heading").removeClass("d_active");
	$(this).addClass("d_active");

	$("ul.tabs li").removeClass("active");
	$("ul.tabs li[rel^='" + d_activeTab + "']").addClass("active");
});

// toggle hidden content
$(".readMoreBtn").click(function () {
	// alert($(this).siblings('.hiddenContent'))
	$(this).siblings(".hiddenContent").toggleClass("active");
	let val = $(this).html();
	val == "+" ? $(this).html("-") : $(this).html("+");
});

// swiper with vertical textual pagination
var init = false;
var vertSlider;
var menu = [];
$(".vert-swipper .tab_drawer_heading").each(function () { menu.push($(this).text()) });
console.log(menu)
// var menu = ["Customer Acquisition", "Customer Management", "Collections", "Business Planning", "Process Automation", "Due Diligence", "Digital Transformation"];
function vertSwiperCard() {
	if (window.innerWidth >= 768) {
		if (!init) {
			init = true;
			vertSlider = new Swiper(".vert-swipper", {
				lazy: true,
				loop: true,
				speed: 1000,
				slidesPerView: 1.5,
				spaceBetween: 40,
				// autoplay: {
				//    delay: 1000,
				//    disableOnInteraction: false,
				// },
				// If we need pagination
				pagination: {
					el: ".vert-pagination",
					clickable: true,
					renderBullet: function (index, className) {
						return '<li class="' + className + '">' + menu[index] + "</li>";
					},
				},
			});
		}
	} else if (init) {
		vertSlider.destroy();
		init = false;
	}
}
vertSwiperCard();
window.addEventListener("resize", vertSwiperCard);
