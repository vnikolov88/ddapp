/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass");

gulp.task('default', ['styles', 'styles:min', 'code']);

var apps = ['ddz', 'smarthelp', 'preventicum', 'marienschwerte', 'sesnothelfer'];

gulp.task('styles', function () {
    apps.map(function(app) {
        return gulp.src('Apps/' + app + '/Styles/Main.scss', { sourcemaps: true })
            .pipe(sass())
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
    apps.map(function(app) {
        return gulp.src('Apps/' + app + '/Code/*.js')
            .pipe(concat('./app.min.js'))
            .pipe(uglify())
            .pipe(gulp.dest('wwwroot/' + app + '/'));
    });
});
