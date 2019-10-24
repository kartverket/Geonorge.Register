const gulp = require("gulp")

gulp.task('copy-felles', function () {
  return gulp.src('./node_modules/geonorge-base/assets/**/*').pipe(
    gulp.dest('./Content/bower_components/kartverket-felleskomponenter/assets/')
  )
})

gulp.task('copy-shared', function () {
  return gulp.src('./node_modules/geonorge-shared-partials/dist/*').pipe(
    gulp.dest('./dist/')
  )
})

gulp.task('default', gulp.series('copy-felles', 'copy-shared'))