$("#dateFrom").datetimepicker();
$("#dateTo").datetimepicker();

function removeFilter() {
	$("#dateFrom").val('');
	$("#dateTo").val('');
	$("#searchForm").submit();
}