/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    gulpif = require("gulp-if"),
    concat = require("gulp-concat"),
    minify = require("gulp-babel-minify"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    folders = require("gulp-folders"),
    sass = require("gulp-sass");

gulp.task('default', ['styles', 'styles:min', 'code', 'code:min']);

var appsRoot = 'Apps';

gulp.task('styles', folders(appsRoot, function (app) {
    return gulp.src([
        'wwwroot/css/fonts.css',
        'wwwroot/css/gallery.css',
        appsRoot + '/' + app + '/Styles/Main.scss'
    ], { sourcemaps: true })
        .pipe(gulpif(/[.]scss$/, sass()))
        .pipe(concat('./app.css'))
        .pipe(gulp.dest('wwwroot/' + app + '/', { sourcemaps: true }));
}));

gulp.task('styles:min', folders(appsRoot, function (app) {
    return gulp.src('wwwroot/' + app + '/app.css')
        .pipe(cssmin())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('wwwroot/' + app + '/'));
}));

gulp.task('watch', function () {
    folders(appsRoot, function (app) {
        gulp.watch(appsRoot + '/' + app + '/Styles/*.scss', ['styles']);
    });

    gulp.watch(appsRoot + '/global/Styles/*.scss', ['styles']); 
});

gulp.task('code', folders(appsRoot, function (app) {
    return gulp.src([
        'wwwroot/js/page-load.js',
        'wwwroot/js/gallery.js',
        'wwwroot/js/nav-bar.js',
        'wwwroot/js/nugget.js',
        'wwwroot/js/vue.js',
        appsRoot + '/global/Code/*.js',
        appsRoot + '/' + app + '/Code/*.js'
    ])
        .pipe(concat('./app.js'))
        .pipe(gulp.dest('wwwroot/' + app + '/'));
}));

gulp.task('code:min', folders(appsRoot, function (app) {
    return gulp.src('wwwroot/' + app + '/app.js')
        .pipe(minify().on('error', function (e) {
            console.log(e);
        }))
        .pipe(rename("app.min.js"))
        .pipe(gulp.dest('wwwroot/' + app + '/'));
}));
