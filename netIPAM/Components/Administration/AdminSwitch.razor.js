export function createSwitchButton(switchBtnContainer, switchBtnOption) {
    const DEFAULTS = {
        onlabel: 'On',
        onstyle: 'primary',
        offlabel: 'Off',
        offstyle: 'default',
        size: 'mini',
        style: '',
        width: null,
        height: null,
    };
    switchBtnOption = switchBtnOption || {};
    let options = {
        onlabel: switchBtnOption.onlabel || DEFAULTS.onlabel,
        onstyle: switchBtnOption.onstyle || DEFAULTS.onstyle,
        offlabel: switchBtnOption.offlabel || DEFAULTS.offlabel,
        offstyle: (switchBtnOption.offstyle === 'light' ? 'default' : switchBtnOption.offstyle) || DEFAULTS.offstyle,
        size: switchBtnOption.size || DEFAULTS.size,
        style: switchBtnOption.style || DEFAULTS.style,
        width: switchBtnOption.width || DEFAULTS.width,
        height: switchBtnOption.height || DEFAULTS.height,
    }

    let input = document.createElement('input');
    input.setAttribute('type', 'checkbox');

    switchBtnContainer.appendChild(input);

    // use bootstrapSwitch (jQuery plugin)
    $(input).bootstrapSwitch({
        onText: options.onlabel,
        offText: options.offlabel,
        onColor: options.onstyle,
        offColor: options.offstyle,
        size: options.size
    });

    return input;
}

export function setSwitchButtonStatus(checkBoxInput, status, preventEventPropagate) {
    // bootstrapSwitch uses 'state' (true/false)
    const state = (status === 'on');
    $(checkBoxInput).bootstrapSwitch('state', state, preventEventPropagate);
}

let dotnetInvokeRef = function () {
    // noinspection JSUnusedGlobalSymbols
    return {
        init: function (dotnetInvokeReference, checkBoxReference) {
            $(checkBoxReference).on('switchChange.bootstrapSwitch', function (event, state) {
                const changeEvent = {
                    value: state
                };
                // noinspection JSUnresolvedFunction
                dotnetInvokeReference.invokeMethodAsync('SwitchBtnEventHandler', changeEvent)
            });
        }
    };
}

// noinspection JSUnusedGlobalSymbols
export let createDotNetInvokeRef = function () {
    return new dotnetInvokeRef();
};