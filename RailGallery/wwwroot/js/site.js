// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $(".form-control-chosen").chosen({
        search_contains: true,
        display_selected_options: true,
        display_disabled_options: true
    });
});