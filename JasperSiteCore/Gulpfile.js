const gulp = require('gulp');

gulp.task('default', function () {
    return gulp.src(['node_modules/tinymce/**/*', 'node_modules/jquery-ui-dist/**/*'], {
        base: 'node_modules'
    }).pipe(gulp.dest('wwwroot/nodeLib'));
});