/* Envi Kendo Grid Table
 * Device: PC/Tablet
-----------------------------------------------------------------------------------------------------------*/

$.fn.kendoList = function () {

	// Get list element data
	var listElementData = {};
	listElementData.list = $(this);

	var methods = {

		initGrid: function () {

			listElementData.listId = listElementData.list.attr("id");
			listElementData.readUrl = listElementData.list.attr("data-url");
			listElementData.updateUrl = listElementData.list.data("update-url") || "";
			listElementData.scrollable = listElementData.list.attr("data-scrollable");
			listElementData.scrollable = (typeof listElementData.scrollable !== "undefined") ? listElementData.scrollable : false;
			listElementData.scroll = listElementData.list.data("scroll");
			listElementData.scroll = (typeof listElementData.scroll !== "undefined") ? listElementData.scroll : false;
			listElementData.resizable = listElementData.list.attr("data-resizable");
			listElementData.resizable = (typeof listElementData.resizable !== "undefined") ? listElementData.resizable : false;
			listElementData.sortable = listElementData.list.attr("data-sortable");
			listElementData.sortable = (typeof listElementData.sortable !== "undefined" && listElementData.sortable == "false") ? false : true;
			listElementData.editable = listElementData.list.data("editable");
			listElementData.editable = (typeof listElementData.editable !== "undefined") ? "inline" : false;
			listElementData.navigatable = listElementData.list.data("navigatable");
			listElementData.navigatable = (typeof listElementData.navigatable !== "undefined") ? listElementData.navigatable : false;
			listElementData.template = listElementData.list.data("template");
			listElementData.rowClickType = listElementData.list.attr("data-row-type");
			listElementData.rowClickType = (listElementData.rowClickType == "selection") ? "selection" : "transition";
			listElementData.sortField = listElementData.list.find("th[data-sort=sortable]").data("field");
			listElementData.sortDirection = listElementData.list.find("th[data-dir]").data("dir");
			listElementData.sortDirection = (typeof listElementData.sortDirection !== "undefined") ? listElementData.sortDirection : "asc";
			listElementData.inactiveButton = $("a.toggle_inactive[data-grid=" + listElementData.listId + "]");
			listElementData.searchInput = $("input[name=search][data-grid=" + listElementData.listId + "]");
			listElementData.disableSpace = listElementData.searchInput.data("disable-space");
			listElementData.disableSpace = (typeof listElementData.disableSpace !== "undefined" && listElementData.disableSpace != "false") ? true : false;
			listElementData.filteringOptions = $("a.filtering_options[data-grid=" + listElementData.listId + "]");
			listElementData.filterBox = listElementData.filteringOptions.parent();
			listElementData.filterBlockMenu = $("#" + listElementData.filteringOptions.attr("data-grid") + "_filter");
			listElementData.pageSize = listElementData.list.attr("data-page-size") || 20;
			listElementData.hideOnEmptyGrid = $(".hide_on_empty_" + listElementData.listId);
			listElementData.hideOnEmptyGrid.hide();
			listElementData.loadInitial = listElementData.list.attr("data-loadInitial") != 'false';
			listElementData.schemaModel = {};
			listElementData.buttons = $("a.envi_grey_button[data-grid=" + listElementData.listId + "][data-filter='true']");
			
			// Is storage accepted for this list id
			if (methods.isStorageAcceptedForThisList()) {
				listElementData.storageParams = methods.getFiltersFromStorage();
			} else {
				listElementData.storageParams = null;
			}
			listElementData.firstLoadFlag = true;
			
			// Use template or not
			if ((typeof listElementData.template === "undefined")) {
				listElementData.rowTemplate = kendo.template(listElementData.list.siblings("script#rowTemplate").html());
			} else {
				listElementData.rowTemplate = false;
			}

			// Set fields params to schema model
			if ($.isFunction(listElementData.fieldParams)) {
				listElementData.schemaModel = listElementData.fieldParams();
			}

			listElementData.list.kendoGrid({
				autoBind: listElementData.loadInitial,
				dataBound: methods.onDataBound,
				dataSource: {
					transport: {
						read: function (options) {

							var that = this;
							
							$.ajax({
									url: listElementData.readUrl,
									dataType: "json",
									data: that.parameterMap(options, "read"),
									//cache: false,
									//headers: { "cache-control": "no-cache" },
									type: "POST",
									success: function (data) {

										// Remember status code of response to global variable.
										listElementData.statusCode = data.StatusCode;

										// Call kendo success method
										options.success(data);

										// Set system message from server
										if (data.Count == 0) {
											methods.setServerMessage(data.Message);
										}
									}
								});
							},
						update: function (options) {

							var that = this;
							
							$.ajax({
								url: listElementData.updateUrl,
								dataType: "json",
								contentType: "application/json",
								data: that.parameterMap(options, "update"),
								cache: false,
								headers: { "cache-control": "no-cache" },
								type: "POST",
								success: function (response) {

									// Show error validation message
									if (response.StatusCode == Envi.Enums.AjaxStatusCode.Error) {

										$.fn.showMessage(response.Message);
									} else {
										
										// Call kendo success method
										options.success(response);
									}
								}
							});
						},
						parameterMap: function (options, operation) {

							// Add models params on update method
							if (operation !== "read" && options.data.models) {
								return JSON.stringify(options.data);
							}

							var parameters;

							// Is storage accepted for this list id
							if (methods.isStorageAcceptedForThisList()) {

								// Get params from storage if it exist				
								if (listElementData.storageParams != null && listElementData.firstLoadFlag) {

									// Load from storage one time
									listElementData.firstLoadFlag = false;

									// Set values from storage to filters
									methods.setStorageValuesToFilters();

									// Remove "filterBoxOpened" element from storage array
									parameters = new cloneObject(listElementData.storageParams);
									delete parameters.filterBoxOpened;

									return parameters;
								}
							}
							
							// Get parameters array
							parameters = methods.getParameterFilters(options);

							// Add extra filters
							parameters = methods.addExtraFiltersToArray(parameters);

							// Save array in storage
							methods.saveFiltersToStorage(parameters);

							// Remove "filterBoxOpened" element from parameters array 
							delete parameters.filterBoxOpened;

							return parameters;
						}
					},
					batch: listElementData.editable,
					schema: {
						data: "Items",
						total: "Count",
						message: "Message",
						model: listElementData.schemaModel
					},
					change: function (data) {

						// If action is item change - return
						if (data.action == "itemchange") {
							return;
						}

						// Get data
						var statusCode = listElementData.statusCode;
						var itemsPerPage = listElementData.list.data().kendoGrid.dataSource.view().length;
						var currentPage = listElementData.list.data().kendoGrid.dataSource.page();
						var page;

						// Scroll up to the header
						methods.scrollToTopOfList();

						// If items per page is 0 - redirect to -1 page
						if (itemsPerPage == 0 && currentPage != 1) {
							page = currentPage - 1;
						}

						if (page != "" && typeof (page) !== "undefined") {
							listElementData.list.data().kendoGrid.dataSource.page(page);
							return;
						}

						// If total result is 0 - hide controll buttons
						if (data.sender.total() == 0) {
							methods.toggleExtraButtons("disable", statusCode);
						} else {
							methods.toggleExtraButtons("enable", statusCode);
						}

						// Find paginator
						listElementData.pagination = listElementData.list.siblings(".k-pager-wrap");

						// Hide pager if total items count less than pageSize
						if (data.sender.total() <= data.sender.pageSize()) {
							listElementData.pagination.hide();
						} else {
							listElementData.pagination.show();
						}

						// Hide buttons if grid without items
						if (data.sender.total() <= 0) {
							listElementData.hideOnEmptyGrid.hide();
						} else {
							listElementData.hideOnEmptyGrid.show();
						}

						// Uncheck all checkboxes between table actions, but not for Modules->PO grid
						listElementData.list.find("input.styled[type=checkbox]").attr('checked', false);
						listElementData.list.find("span.checkbox").css({ backgroundPosition: '0 0' });

						// Set checkbox true to PO Grid
						if (listElementData.listId == "po_list") {
							listElementData.list.find("input.styled[type=checkbox]").attr('checked', true);
							listElementData.list.find("span.checkbox").css({ backgroundPosition: '0 -34px' });
						}

						// Workaround: init elements after timeout
						setTimeout(function () {

							// Call callback
							if ($.isFunction(listElementData.callback)) {
								listElementData.callback();
							}

							// Init selectboxes
							if ($.fn.initCombobox) {
								$.fn.initCombobox(listElementData.list.find("select"));
							}

							// Init textboxes
							listElementData.list.find("input[type=text]").textinput();

							listElementData.rowItem = listElementData.list.find("tbody tr");
							listElementData.rowItem.off("click");
							listElementData.rowItem.on("click", methods._openItemClick);

							listElementData.cellItem = listElementData.list.find("tbody td");

							if ($.fn.isPC()) {
								listElementData.cellItem.off("mouseenter");
								listElementData.cellItem.on("mouseenter", methods._initTooltipForTrimmedText);
							}
							
							listElementData.searchInput = $("input[name=search][data-grid=" + listElementData.listId + "]")
							listElementData.searchInput.off("keypress");
							listElementData.searchInput.on("keypress", methods._onSearchInputKeypress);

							listElementData.searchClearButton = listElementData.searchInput.siblings("a.ui-input-clear");
							listElementData.searchClearButton.on("click", methods._onResetSearchButtonClick);

							listElementData.kendoGridData = listElementData.list.data("kendoGrid");

							listElementData.inactiveButton.off("click");
							listElementData.inactiveButton.on("click", methods._onToggleInactiveClick);

							listElementData.filteringOptions.off("click");
							listElementData.filteringOptions.on("click", methods._toggleFilteringOptions);

							listElementData.headerCheckbox = listElementData.list.find(".checkbox_header span.checkbox");

							listElementData.headerCheckbox.off("mouseup");
							listElementData.headerCheckbox.on("mouseup", methods._onSelectAllCheckboxClick);

							listElementData.rowCheckbox = listElementData.list.find(".checkboxBlock span.checkbox");

							listElementData.rowCheckbox.off("mouseup");
							listElementData.rowCheckbox.on("mouseup", methods._onUncheckSelectAllCheckbox);
							
							// Toggle button for "show/hide" state
							listElementData.buttons.off("click");
							listElementData.buttons.on("click", methods.toggleShowHideState);

							if (data.sender.total() == 0 && listElementData.list.attr("data-loadinitial") == "false") {
								methods.setServerMessage(Envi.resources.core.txtNoRecordsFound);
							}							
						}, 1);

						if (listElementData.kendoGridData
							&& listElementData.loadInitial // indicates initial loading state
							&& listElementData.sortable // indicates that user able to sort columns
							&& listElementData.kendoGridData.options.sortable == false) {

							listElementData.kendoGridData.options.sortable = true; // Allow sorting
							listElementData.kendoGridData._sortable(); // Initialize possibility to sort allowed column
							listElementData.list.find("th[data-field=" + listElementData.sortField + "]").getKendoSortable(); // Initialize default sorting arrow
						}

						// Add scroll to grid block
						if (listElementData.scroll) {
							
							var gridBlock = listElementData.list.parent();
							
							if (!gridBlock.hasClass("scroll")) {
								gridBlock.addClass("scroll");
							}
						}

						// Indicates that data can be loaded and sorting can be allowed
						listElementData.loadInitial = true;
					},
					page: (listElementData.storageParams != null) ? listElementData.storageParams.page : 1,
					pageSize: (listElementData.storageParams != null) ? listElementData.storageParams.pageSize : listElementData.pageSize,
					sort: {
						field: (listElementData.storageParams != null) ? listElementData.storageParams.sortBy : listElementData.sortField,
						dir: (listElementData.storageParams != null) ? listElementData.storageParams.sortDir : listElementData.sortDirection,
					},
					serverPaging: true,
					serverSorting: true,
					serverFiltering: true,
				},
				rowTemplate: listElementData.rowTemplate,
				sortable: listElementData.loadInitial && listElementData.sortable,
				pageable: true,
				scrollable: listElementData.scrollable,
				resizable: listElementData.resizable,
				editable: listElementData.editable,
				navigatable: listElementData.navigatable,
				columns: ($.isFunction(listElementData.columns)) ? listElementData.columns() : []
			});
			
			if ($.fn.isPC()) {

				// Init kendoTooltip plugin for show tooltips on trimmed td's
				listElementData.list.kendoTooltip({
					filter: "td.showTooltip",
					restoreTitle: false,
					content: function(e) {
						return $(e.target).text();
					}
				});
			}
			
			// Adjust horizontal scroll
			methods._adjustHorizontalScroll();
		},
		isStorageAcceptedForThisList: function () {

			var access = false;

			switch (listElementData.listId) {
				case Envi.Enums.Lists.UserList:
				case Envi.Enums.Lists.POList:
				case Envi.Enums.Lists.POSearchList:
				case Envi.Enums.Lists.LineItemsList:
				case Envi.Enums.Lists.AddFromPOList:
				case Envi.Enums.Lists.AddFromPOLIList:
				case Envi.Enums.Lists.FacilityList:
				case Envi.Enums.Lists.AddFacilityList:
				case Envi.Enums.Lists.UserOrganizationsList:
				case Envi.Enums.Lists.FacilitySecurityList:
				case Envi.Enums.Lists.OrderEntryList:
				case Envi.Enums.Lists.OELineItemsList:
					access = true;
					break;
				default:
					break;
			}

			return access;
		},
		getFiltersFromStorage: function () {

			// Read filter array from storage
			return readStorageRecord(listElementData.listId);
		},
		saveFiltersToStorage: function (listFilters) {

			// Set to the storage
			saveStorageRecord(listElementData.listId, listFilters);
		},
		getParameterFilters: function (options) {
			
			// Get sort params
			var sortParams = {};
			listElementData.sortField = (typeof listElementData.sortField != "undefined") ?
										listElementData.sortField :
										listElementData.list.find("th[data-field]:first").attr("data-field");

			if (options.data.sort != null && options.data.sort[0] != null) {
				sortParams = { sortBy: options.data.sort[0].field, sortDir: options.data.sort[0].dir };
			} else {
				sortParams = { sortBy: listElementData.sortField, sortDir: listElementData.sortDirection };
			}

			// Perform parameters
			var parameters = {
				sortBy: sortParams.sortBy,
				sortDir: sortParams.sortDir,
				page: options.data.page,
				pageSize: options.data.pageSize,
				search: listElementData.searchInput.val(),
				includeInactive: methods.getInactiveButtonState(),
				filterBoxOpened: (listElementData.filterBox.hasClass("on")) ? true : false
			};

			return parameters;
		},
		addExtraFiltersToArray: function (listFilters) {

			var extraFilters;

			if ($.isFunction(listElementData.getFilters)) {
				extraFilters = listElementData.getFilters();
			}

			// Push extra filter parameters
			if (!$.isEmptyObject(extraFilters)) {
				$.each(extraFilters, function (index, value) {
					listFilters[index] = value;
				});
			}

			return listFilters;
		},
		setStorageValuesToFilters: function () {

			var currentFilter;

			// Search input
			listElementData.searchInput.val(listElementData.storageParams.search);

			// "Display inactive" button
			if (listElementData.storageParams.includeInactive) {
				methods.displayInactive();
			} else {
				methods.hideInactive();
			}

			// Filter box
			if (listElementData.storageParams.filterBoxOpened) {

				// Add "on" class
				listElementData.filterBox.addClass("on");

				// Open filter block menu
				listElementData.filterBlockMenu.slideDown("fast");
			}

			// Selectboxes/checkboxes
			$.each(listElementData.storageParams, function (index, value) {

				currentFilter = $("#" + index);
				
				if (currentFilter.is("select")) {
					$.fn.setComboboxValue(currentFilter, value);
				} else if (currentFilter.is(":checkbox")) {
					currentFilter.prop("checked", value);
					currentFilter.refreshCheckbox();
				}
			});
			
			// Buttons
			$.each(listElementData.buttons, function (index, value) {
				methods.setStorageStateForButton($(this));
			});
		},
		setServerMessage: function (message) {

			var colCount = listElementData.list.find("th").size();
			listElementData.list.find("tbody").html("<tr><td class='no_records' colspan=" + colCount + ">" + message + "</td></tr>");
		},
		setStorageStateForButton: function (button) {
			
			var buttonId = button.attr("id");
			var buttonLabelOn;
			var buttonLabelOff;
			var hiddenInput;

			if (listElementData.storageParams != null && listElementData.storageParams[buttonId] != null) {

				buttonLabelOn = button.children(".on");
				buttonLabelOff = button.children(".off");
				hiddenInput = button.children("input[type=hidden]");

				// Check for state value
				if (listElementData.storageParams[buttonId] == "true") {
					buttonLabelOn.addClass("hide");
					buttonLabelOff.removeClass("hide");
					hiddenInput.val("true");
				} else if (listElementData.storageParams[buttonId] == "false") {
					buttonLabelOn.removeClass("hide");
					buttonLabelOff.addClass("hide");
					hiddenInput.val("false");
				}
			}
		},
		toggleShowHideState: function (event) {

			event.preventDefault();

			var button = $(this);
			var hiddenInput = button.children("input[type=hidden]");

			// Toggle class "hide"
			button.children(".text").toggleClass("hide");

			// Change value for hidden input
			if (hiddenInput.val() == "true") {
				hiddenInput.val("false");
			} else if (hiddenInput.val() == "false") {
				hiddenInput.val("true");
			}
			listElementData.kendoGridData.dataSource.page(1);
		},
		scrollToTopOfList: function () {

			// Scroll up
			listElementData.list.parent(".k-grid").stop().animate({
				scrollTop: 0
			}, "fast");
		},
		getInactiveButtonState: function () {

			var state;

			if (listElementData.inactiveButton.hasClass("off")) {
				state = false;
			} else if (listElementData.inactiveButton.hasClass("on")) {
				state = true;
			}

			return state;
		},
		displayInactive: function () {

			listElementData.inactiveButton.removeClass("off").addClass("on");
			listElementData.inactiveButton.text("Hide Inactive");
		},
		hideInactive: function () {

			listElementData.inactiveButton.removeClass("on").addClass("off");
			listElementData.inactiveButton.text("Display Inactive");
		},
		switchInactiveButton: function () {

			if (listElementData.inactiveButton.hasClass("off")) {
				methods.displayInactive();
			} else if (listElementData.inactiveButton.hasClass("on")) {
				methods.hideInactive();
			}
		},
		toggleExtraButtons: function (buttonState, statusCode) {

			if ($.isFunction(listElementData.extraButtonsAction)) {
				listElementData.extraButtonsAction(buttonState, statusCode);
			}
		},
		clearSearchParameters: function (options) {

			var defaultSortField = listElementData.kendoGridData.options.dataSource.sort;
			var defaultPageSize = listElementData.kendoGridData.options.dataSource.pageSize;
			var itemsCount = listElementData.kendoGridData.dataSource.view().length;
			var currentPage = listElementData.kendoGridData.dataSource.page();
			var page;

			// Refresh search parameters
			if (typeof (options.refreshSearchParams) === "undefined" || options.refreshSearchParams != false) {

				// Clear search value
				listElementData.searchInput.val("");

				// Hide search clear button
				listElementData.searchClearButton.addClass("ui-input-clear-hidden");

				// Set hide inactive class and text
				methods.hideInactive();

				// Hide filtering options box
				if (typeof (options.collapseFilterBox) === "undefined" && options.collapseFilterBox != false) {
					if (listElementData.filterBox.hasClass("on")) {
						listElementData.filterBox.removeClass("on").addClass("off");
						listElementData.filterBlockMenu.hide();
					}
				}
			}

			// Set page
			if (options.itemsChecked == itemsCount && currentPage != 1) {
				page = currentPage - 1;
			} else {
				page = 1;
			}

			// Clear filter/sort/page
			listElementData.kendoGridData.dataSource.query({
				page: page,
				pageSize: defaultPageSize,
				sort: {
					field: defaultSortField.field,
					dir: defaultSortField.dir
				},
				filter: [
					{ field: "Search", value: listElementData.searchInput.val() },
					{ field: "Status", value: methods.getInactiveButtonState() }
				]
			});
		},
		_adjustHorizontalScroll: function () {

			if ($.browser.msie && $.browser.version == 9) {
				$("[data-role=resizable]").css("overflow", "visible", "!important");
			}
		},
		_onToggleInactiveClick: function () {

			// Toggle "Inactive" button
			methods.switchInactiveButton();

			// Refresh the list
			listElementData.kendoGridData.dataSource.page(1);
		},
		_onSearchInputKeypress: function (e) {

			// On "Enter" key press
			if (e.keyCode == 13) {

				if (listElementData.disableSpace) {

					var clearedValue = $(this).val().replace(/ /g, "");

					if (!clearedValue.length) {
						return false;
					}
				}

				// Hide keyboard on Tablet
				if ($.fn.isTablet()) {
					$(this).blur();
				}

				// Refresh the list
				listElementData.kendoGridData.dataSource.page(1);
			}
		},
		_onResetSearchButtonClick: function () {

			// Hide keyboard on Tablet
			if ($.fn.isTablet()) {
				listElementData.searchInput.blur();
			}

			// Refresh the list
			listElementData.kendoGridData.dataSource.page(1);
		},
		_toggleFilteringOptions: function () {

			// Toggle classes on/off
			listElementData.filterBox.toggleClass("off").toggleClass("on");

			// Open filter block menu
			listElementData.filterBlockMenu.slideToggle("fast");

			// Save filter box state to storage
			var parameters = methods.getFiltersFromStorage();
			parameters.filterBoxOpened = (listElementData.filterBox.hasClass("on")) ? true : false;
			methods.saveFiltersToStorage(parameters);
		},
		_openItemClick: function (e) {

			var row = $(this);

			// Disable click when grid is empty
			if (row.find("td.no_records").length) {
				return false;
			}

			// Row click type behavior
			switch (listElementData.rowClickType) {

				case "selection":
					listElementData.rowItem.removeClass("selected");
					$(this).addClass("selected");

					break;

				case "transition":

					// Get DOM data
					var hrefLink = row.attr("href");
					var dataSourceLink = row.attr("data-source");
					var targetClassName = e.target.className;

					// If row has no class "disable"
					if (!row.hasClass("disable")) {

						// If clicked target is not a checkbox
						if (targetClassName != "checkbox") {

							// Hide tooltips when go out from grid page
							if (typeof (listElementData.list.data("kendoTooltip")) !== "undefined") {
								listElementData.list.data("kendoTooltip").destroy();
							}

							// If href has no "undefined" value
							if (typeof hrefLink !== "undefined" && hrefLink !== false) {

								// Change page
								$.fn.enviNavigate(hrefLink, { reloadPage: true });

								// Add class disable
								row.addClass("disable");

							} else if (typeof dataSourceLink !== "undefined") {

								// Activate line item child tab
								row.activateChildTab();
							}
						}
					}

					break;
			}
		},
		_initTooltipForTrimmedText: function () {

			// Add class for td's where text overflow ellipsis and need show tooltip
			var dummyCell = $(this)
                    .clone()
					.text($.trim($(this).text()))
                    .css({ display: "inline", width: "auto", visibility: "hidden", padding: "0px", margin: "0px", border: "0px", fontSize: "14px" })
                    .appendTo("body"); 

			if ((dummyCell.width() > $(this).width()) && ( !($(this).find("select, input, checkbox, textarea").length) ) ) {
				$(this).addClass("showTooltip");
			} else {
				$(this).removeClass("showTooltip");
			}

			dummyCell.remove();
		},
		_onSelectAllCheckboxClick: function () {

			if (!listElementData.headerCheckbox.hasClass("disabled")) {
				var getAllCheckboxes = listElementData.list.find("tbody input[type=checkbox]");
				if (listElementData.headerCheckbox.siblings("[type=checkbox]").is(':checked')) {
					getAllCheckboxes.attr('checked', 'checked');
				} else {
					getAllCheckboxes.removeAttr('checked');
				}
			}
		},
		_onUncheckSelectAllCheckbox: function () {

			var table = listElementData.list;
			var selectAllCheckbox = table.find("#select_all");
			var itemsChecked = table.find("tbody input[type=checkbox]:checked").size();
			var itemsCountOnPage = table.data("kendoGrid").dataSource.view().length;

			if (itemsChecked == itemsCountOnPage) {

				// Check main
				selectAllCheckbox.attr("checked", "checked");
			} else {

				// Uncheck main
				selectAllCheckbox.removeAttr("checked");
			}
		}
	};


	// Return element data and methods
	return { "listElementData": listElementData, "methods": methods };
};