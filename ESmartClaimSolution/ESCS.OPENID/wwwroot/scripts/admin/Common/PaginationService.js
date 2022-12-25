//create by: thanhnx.vbi
function PaginationService(idPagination, onPageClick = undefined) {
    this.id = idPagination;
    this.init = function () {
        $("#" + this.id).pagination({
            items: 0,
            itemsOnPage: 20,
            cssStyle: 'light-theme',
            onPageClick: onPageClick
        });
    };
    this.updateItems = function (totalItems) {
        $("#" + this.id).pagination('updateItems', totalItems);
    }
    this.init();
}