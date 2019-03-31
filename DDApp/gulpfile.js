/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    cssmin = require("gulp-cssmin"),
    sass = require("gulp-sass");

gulp.task('default', ['styles', 'code']);

var apps = ['ddz', 'smarthelp', 'preventicum'];

gulp.task('styles', function () {
    apps.map(function(app) {
        return gulp.src('Apps/' + app + '/Styles/*.scss')
            .pipe(sass())
            .pipe(concat('./app.min.css'))
            .pipe(cssmin())
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
