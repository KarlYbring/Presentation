
//CHATGPT GENERERAD & SCRIPT FRÅN LEKTIONER OCH INSPLENINGAR

window.addEventListener('resize', removeSidebarShowOnResize);
document.addEventListener('DOMContentLoaded', () => {
    initCloseButtons();
    initMobileMenu();
    initModals();
    initializeDropdowns();
    updateRelativeTimes();
    setInterval(updateRelativeTimes, 60000);
    setupEditProjectButtons();
});

function initMobileMenu() {
    document.querySelectorAll('[data-type="menu"]').forEach(button => {
        button.addEventListener('click', () => {
            const target = document.querySelector(button.getAttribute('data-target'));
            target?.classList.add('show');
        });
    });
}

function initModals() {
    document.querySelectorAll('[data-type="modal"]').forEach(button => {
        button.addEventListener('click', () => {
            const target = document.querySelector(button.getAttribute('data-target'));
            target?.classList.add('show');
        });
    });
}

function initCloseButtons() {
    document.querySelectorAll('[data-type="close"]').forEach(button => {
        button.addEventListener('click', () => {
            const target = document.querySelector(button.getAttribute('data-target'));
            target?.classList.remove('show');
        });
    });
}

function removeSidebarShowOnResize() {
    const sidebar = document.getElementById('sidebar');
    if (sidebar?.classList.contains('show')) {
        sidebar.classList.remove('show');
    }
}

function initializeDropdowns() {
    const dropdownTriggers = document.querySelectorAll('[data-type="dropdown"]');
    const dropdownElements = new Set();

    dropdownTriggers.forEach(trigger => {
        const targetSelector = trigger.getAttribute('data-target');
        const dropdown = document.querySelector(targetSelector);
        if (dropdown) dropdownElements.add(dropdown);

        trigger.addEventListener('click', (e) => {
            e.stopPropagation();
            if (!dropdown) return;
            closeAllDropdowns(dropdown, dropdownElements);
            dropdown.classList.toggle('show');
        });
    });

    dropdownElements.forEach(dropdown => {
        dropdown.addEventListener('click', e => e.stopPropagation());
    });

    document.addEventListener('click', () => {
        closeAllDropdowns(null, dropdownElements);
    });
}

function closeAllDropdowns(exceptDropdown, dropdownElements) {
    dropdownElements.forEach(dropdown => {
        if (dropdown !== exceptDropdown) {
            dropdown.classList.remove('show');
        }
    });
}

function updateRelativeTimes() {
    const elements = document.querySelectorAll('.time');
    const now = new Date();

    elements.forEach(el => {
        const created = new Date(el.getAttribute('data-created'));
        const diff = now - created;
        const mins = Math.floor(diff / 60000);
        const hours = Math.floor(mins / 60);
        const days = Math.floor(hours / 24);
        const weeks = Math.floor(days / 7);

        let text = '';
        if (mins < 1) text = 'Just now';
        else if (mins < 60) text = `${mins} min ago`;
        else if (hours < 2) text = `${hours} hour ago`;
        else if (hours < 24) text = `${hours} hours ago`;
        else if (days < 2) text = `${days} day ago`;
        else if (days < 7) text = `${days} days ago`;
        else text = `${weeks} weeks ago`;

        el.textContent = text;
    });
}

function setupEditProjectButtons() {
    document.querySelectorAll('.dropdown-action[data-target="#editProjectModal"]').forEach(button => {
        button.addEventListener('click', async function () {
            const projectId = this.getAttribute('data-id');
            const container = document.querySelector("#editProjectModalContainer");

            try {
                const response = await fetch(`/Projects/Edit/${projectId}`);
                const html = await response.text();

                // Inject the modal HTML into the DOM
                container.innerHTML = html;

                const modal = document.querySelector("#editProjectModal");
                modal?.classList.add("show");

                // Optional: If you use a WYSIWYG editor
                initWysiwygEditor?.(
                    '#edit-project-description-editor',
                    '#edit-project-description-toolbar',
                    '#edit-project-description',
                    document.querySelector('#edit-project-description')?.value ?? ''
                );

                const form = modal.querySelector("form");
                if (form) {
                    form.addEventListener("submit", async function (e) {
                        e.preventDefault();
                        const formData = new FormData(form);

                        const response = await fetch("/Projects/Update", {
                            method: "POST",
                            body: formData
                        });

                        const result = await response.json();

                        if (result.success) {
                            window.location.href = "/admin/projects";
                        } else {
                            showValidationErrors(result.errors);
                        }
                    });
                }

            } catch (error) {
                console.error("❌ Failed to load edit form:", error);
            }
        });
    });
}
function showValidationErrors(errors) {
    for (const field in errors) {
        const messages = errors[field];
        const input = document.querySelector(`[name="${field}"]`);

        if (input) {
            // Remove any existing error messages first
            const existing = input.parentElement.querySelector(".text-danger");
            if (existing) existing.remove();

            const errorSpan = document.createElement("span");
            errorSpan.className = "text-danger";
            errorSpan.textContent = messages.join(', ');
            input.parentElement.appendChild(errorSpan);
        }
    }
}
(function () {
    try {
        if (localStorage.getItem('theme') === 'dark') {
            document.documentElement.setAttribute('data-mode', 'dark')
        }
    }
    catch { }
})()

document.addEventListener('DOMContentLoaded', () => {
    initDarkMode()
    initDarkToggle()
})

function initDarkMode() {
    if (localStorage.getItem('theme') === 'dark') {
        document.body.classList.add('dark-mode')
        document.documentElement.setAttribute('data-mode', 'dark')

        const darkModeToggle = document.querySelector('#darkModeToggle')
        darkModeToggle.checked = true
    }
}

function initDarkToggle() {
    const toggles = document.querySelectorAll('[data-type="toggle"]')

    toggles.forEach(toggle => {
        const togglefunction = toggle.getAttribute('data-func')

        if (togglefunction === "darkmode") {
            toggle.addEventListener('change', function () {
                if (this.checked) {
                    document.documentElement.setAttribute('data-mode', 'dark')
                    localStorage.setItem('theme', 'dark')
                } else {
                    document.documentElement.removeAttribute('data-mode')
                    localStorage.setItem('theme', 'light')
                }
            });
        }
    })
}

