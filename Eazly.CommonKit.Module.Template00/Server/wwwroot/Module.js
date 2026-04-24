/* Module Script */
window.Eazly = window.Eazly || {};

window.Eazly.Template00 = {

    showMessage: function (msg) {
        alert(msg);
    },

    exportToExcel: function (data, fileName) {
        const worksheet = XLSX.utils.json_to_sheet(data);
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");
        XLSX.writeFile(workbook, fileName + ".xlsx");
    }

};