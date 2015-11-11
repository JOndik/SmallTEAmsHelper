'use strict';

// Declare app level module which depends on views, and components
angular
    .module('app')
    .config(appConfig);
    //.run(appRun);

function appConfig($urlRouterProvider, $httpProvider) {
    /*$httpProvider.defaults.xsrfHeaderName = 'X-CSRF-TOKEN';
    $httpProvider.defaults.xsrfCookieName = 'CSRF-TOKEN';
    $httpProvider.defaults.withCredentials = true;*/

    $urlRouterProvider.otherwise('/');
    //$httpProvider.interceptors.push('authInterceptor');
}
appConfig.$inject = ["$urlRouterProvider", "$httpProvider"];

function appRun($rootScope, $state, $timeout, Auth) {
    $rootScope.setFocusAfterStateChange = setFocusAfterStateChange;

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
        if (toState.requiresLogin && !Auth.isAuthenticated()) {
            $state.transitionTo('login');
            event.preventDefault();
        }
    });

    function setFocusAfterStateChange(elemId) {
        var elem = document.getElementById(elemId);

        $timeout(function() {
            if (elem) {
                elem.focus();
            }
        });
    }
}
appRun.$inject = ["$rootScope", "$state", "$timeout", "Auth"];
