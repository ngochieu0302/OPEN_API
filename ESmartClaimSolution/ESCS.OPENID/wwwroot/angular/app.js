var app = angular.module('AppModule', [
    "ngRoute"
]);
app.controller("HomeController", function ($scope, $rootScope, $location) {
    $rootScope.menu = {
        parent: "",
        children:""
    };
    $scope.activeMenuParent = function (path) {
        console.log($location.path());
        return ($location.path().substr(0, path.length) === path) ? 'top-menu-invisible open' : 'top-menu-invisible';
    };
})
app.config(function ($routeProvider, $locationProvider) {
    $routeProvider
        .when('/moi-truong-phat-trien', {
            templateUrl: "environment/index",
            controller: "environmentController"
        });
    //$locationProvider.html5Mode(true);
});
