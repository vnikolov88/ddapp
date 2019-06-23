/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    gulpif = require("gulp-if"),
    concat = require("gulp-concat"),
    minify = require("gulp-babel-minify"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass");

gulp.task('default', ['styles', 'styles:min', 'code', 'code:min']);

var apps = ['ddz', 'smarthelp', 'preventicum', 'marienschwerte', 'sesnothelfer', 'zzm', 'aps'];

gulp.task('styles', function () {
    apps.map(function(app) {
        return gulp.src([
            'wwwroot/css/fonts.css',
            'wwwroot/css/gallery.css',
            'Apps/' + app + '/Styles/Main.scss'
        ], { sourcemaps: true })
            .pipe(gulpif(/[.]scss$/, sass()))
            .pipe(concat('./app.css'))
            .pipe(gulp.dest('wwwroot/' + app + '/', { sourcemaps: true }));
    });
});

gulp.task('styles:min', function () {
    apps.map(function (app) {
        return gulp.src('wwwroot/' + app + '/app.css')
            .pipe(cssmin())
            .pipe(rename({ suffix: '.min' }))
            .pipe(gulp.dest('wwwroot/' + app + '/'));
    });
});

gulp.task('watch', function () {
    apps.map(function (app) {
        gulp.watch('Apps/' + app + '/Styles/*.scss', ['styles']);
    });

    gulp.watch('Apps/global/Styles/*.scss', ['styles']); 
});

gulp.task('code', function () {
    apps.map(function (app) {
        return gulp.src([
            'wwwroot/js/page-load.js',
            'wwwroot/js/gallery.js',
            'wwwroot/js/nav-bar.js',
            'wwwroot/js/nugget.js',
            'wwwroot/js/vue.js',
            'Apps/global/Code/*.js',
            'Apps/' + app + '/Code/*.js'
        ])
            .pipe(concat('./app.js'))
            .pipe(gulp.dest('wwwroot/' + app + '/'));
    });
});

gulp.task('code:min', function () {
    apps.map(function(app) {
        return gulp.src('wwwroot/' + app + '/app.js')
            .pipe(minify().on('error', function (e) {
                console.log(e);
            }))
            .pipe(rename("app.min.js"))
            .pipe(gulp.dest('wwwroot/' + app + '/'));
    });
});
