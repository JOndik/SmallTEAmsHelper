/**
 * Created by Jakub Ondik on 10.9.2015.
 */
'use strict';

angular
    .module('app.change')
    .factory('Change', changeService);

function changeService($resource, API) {
    return $resource(API + 'changes/:estateId');
}