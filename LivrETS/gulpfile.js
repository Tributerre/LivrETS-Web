/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify"),
  sass = require("gulp-sass");

var webroot = "./wwwroot/";

var paths = {
  js: webroot + "js/**/*.js",
  minJs: webroot + "js/**/*.min.js",
  css: webroot + "css/**/*.css",
  minCss: webroot + "css/**/*.min.css",
  concatJsDest: webroot + "js/site.min.js",
  concatCssDest: webroot + "css/site.min.css",
  sassSrc: webroot + "sass/**/*.scss",
  sassDest: webroot + "css"
};

gulp.task('sass', function() {
    return gulp.src(paths.sassSrc)
        .pipe(sass().on("error", sass.logError))
        .pipe(gulp.dest(paths.sassDest));
});

gulp.task("sass:watch", function() {
    gulp.watch(paths.sassSrc, ['sass']);
});

gulp.task("clean:js", function (cb) {
  rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
  rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
  return gulp.src([paths.js, "!" + paths.minJs], {
    base: "."
  })
    .pipe(concat(paths.concatJsDest))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
  return gulp.src([paths.css, "!" + paths.minCss])
    .pipe(concat(paths.concatCssDest))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);
