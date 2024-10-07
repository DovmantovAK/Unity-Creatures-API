using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Creatures.Database.Editor
{
    public class CreaturesDatabaseWindow : EditorWindow
    {
        private CreaturesDatabaseSO _database;
        private ListView _creatureListView;
        private VisualElement _creatureDetailsPanel;
        private TextField _searchField;

        // Поля для локализации и настройки интерфейса
        private const string menu_item_path = "Window/Databases/Unity" + window_title;
        private const string window_title = "Creatures Database";
        private const string add_creature_button_text = "Добавить существо";
        private const string no_creature_selected_message = "Выберите или создайте существо в поле списка";
        private const string search_field_placeholder = "Поиск";
        private const float list_item_height = 50;
        private const int min_window_width = 400;
        private const int min_window_height = 300;
        private const string creature_id_label = " ID: ";
        private const string creature_name_label = "Имя";
        private const string new_creature_name = "Новое существо";
        private const string creature_health_label = "Здоровье";
        private const string creature_immortal_label = "Бессмертное";
        private const string creature_portrait_label = "Портрет";
        private const string creature_animator_label = "Аниматор";
        private const string creature_base_speed_label = "Базовая скорость";
        private const string creature_sprint_multiplier_label = "Множитель спринта";
        

        // Для MakeCreatureListItem
        private const string delete_button_text = "Удалить";
        private const float portrait_width = 50;
        private const float portrait_height = 50;
        
        private static Creature NewCreature => new (id: System.Guid.NewGuid().ToString(),
            creatureName: new_creature_name, health: 100, baseSpeed: 5, sprintMultiplier: 1.5f);


        [MenuItem(menu_item_path)]
        public static void ShowWindow()
        {
            var window = GetWindow<CreaturesDatabaseWindow>(window_title);
            window.minSize = new Vector2(min_window_width, min_window_height);
        }

        public void CreateGUI()
        {
            _database = CreaturesDatabaseSO.Instance;

            var horizontalContainer = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(horizontalContainer);

            // Левая панель - список существ с поиском и кнопкой "Добавить"
            var leftPanel = new VisualElement();
            horizontalContainer.Add(leftPanel);

            // Правая панель - детали выбранного существа
            _creatureDetailsPanel = new VisualElement();
            horizontalContainer.Add(_creatureDetailsPanel);

            // Поисковое поле
            _searchField = new TextField(search_field_placeholder);
            _searchField.RegisterValueChangedCallback(evt => FilterCreatures(evt.newValue));
            leftPanel.Add(_searchField);

            // Кнопка "Добавить существо"
            var addButton = new Button(() => AddCreature()) { text = add_creature_button_text };
            leftPanel.Add(addButton);

            // Список существ
            _creatureListView = new ListView();
            _creatureListView.itemsSource = new List<Creature>(_database.Creatures);
            _creatureListView.fixedItemHeight = (int)list_item_height;
            _creatureListView.selectionType = SelectionType.Single;
            _creatureListView.makeItem = MakeCreatureListItem;
            _creatureListView.bindItem = BindCreatureListItem;
            _creatureListView.selectionChanged += OnCreatureSelected;
            leftPanel.Add(_creatureListView);

            ShowNoCreatureSelectedMessage();
        }

        private void ShowNoCreatureSelectedMessage()
        {
            _creatureDetailsPanel.Clear();
            var noCreatureLabel = new Label(no_creature_selected_message);
            _creatureDetailsPanel.Add(noCreatureLabel);
        }

        private void OnCreatureSelected(IEnumerable<object> selectedItems)
        {
            if (selectedItems == null || selectedItems is not List<object> creatureList || creatureList.Count == 0)
            {
                ShowNoCreatureSelectedMessage();
                return;
            }

            var selectedCreature = (Creature)creatureList[0];
            ShowCreatureDetails(selectedCreature);
            RefreshCreatureList();
        }

        private void ShowCreatureDetails(Creature selectedCreature)
        {
            _creatureDetailsPanel.Clear();

            var idLabel = new Label(creature_id_label + selectedCreature.Id);
            _creatureDetailsPanel.Add(idLabel);

            var nameField = new TextField(creature_name_label) { value = selectedCreature.CreatureName };
            nameField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.CreatureName = evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
                RefreshCreatureList();
            });
            _creatureDetailsPanel.Add(nameField);

            var healthField = new FloatField(creature_health_label) { value = selectedCreature.Health };
            healthField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.Health = evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
            });
            _creatureDetailsPanel.Add(healthField);

            var immortalField = new Toggle(creature_immortal_label) { value = selectedCreature.IsImmortal };
            immortalField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.IsImmortal = evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
            });
            _creatureDetailsPanel.Add(immortalField);

            var portraitField = new ObjectField(creature_portrait_label) { objectType = typeof(Sprite), value = selectedCreature.Portrait };
            portraitField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.Portrait = (Sprite)evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
                RefreshCreatureList();
            });
            _creatureDetailsPanel.Add(portraitField);

            var animatorField = new ObjectField(creature_animator_label) { objectType = typeof(RuntimeAnimatorController), value = selectedCreature.AnimatorController };
            animatorField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.AnimatorController = (RuntimeAnimatorController)evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
            });
            _creatureDetailsPanel.Add(animatorField);

            var baseSpeedField = new FloatField(creature_base_speed_label) { value = selectedCreature.BaseSpeed };
            baseSpeedField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.BaseSpeed = evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
            });
            _creatureDetailsPanel.Add(baseSpeedField);

            var sprintMultiplierField = new FloatField(creature_sprint_multiplier_label) { value = selectedCreature.SprintMultiplier };
            sprintMultiplierField.RegisterValueChangedCallback(evt =>
            {
                var updatedCreature = selectedCreature;
                updatedCreature.SprintMultiplier = evt.newValue;
                _database.UpdateCreatureValues(selectedCreature.Id, updatedCreature);
            });
            _creatureDetailsPanel.Add(sprintMultiplierField);
        }

        private void AddCreature()
        {
            var newCreature = NewCreature;

            _database.AddCreature(newCreature);
            RefreshCreatureList();
        }

        private void RemoveCreature(string creatureId)
        {
            _database.RemoveCreature(creatureId);
            RefreshCreatureList();
        }

        private void RefreshCreatureList()
        {
            _creatureListView.itemsSource = new List<Creature>(_database.Creatures);
            _creatureListView.Rebuild();
        }

        private void FilterCreatures(string searchTerm)
        {
            var filteredCreatures = _database.Creatures
                .Where(creature => creature.CreatureName.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            _creatureListView.itemsSource = filteredCreatures;
            _creatureListView.Rebuild();
        }

        private VisualElement MakeCreatureListItem()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            var portrait = new Image { scaleMode = ScaleMode.ScaleToFit };
            portrait.style.width = portrait_width;
            portrait.style.height = portrait_height;
            container.Add(portrait);

            var nameLabel = new Label();
            nameLabel.style.flexGrow = 1;
            container.Add(nameLabel);

            var deleteButton = new Button(() => { }) { text = delete_button_text };
            deleteButton.style.display = DisplayStyle.None;
            container.Add(deleteButton);

            container.RegisterCallback<MouseEnterEvent>(evt => deleteButton.style.display = DisplayStyle.Flex);
            container.RegisterCallback<MouseLeaveEvent>(evt => deleteButton.style.display = DisplayStyle.None);
            return container;
        }

        private void BindCreatureListItem(VisualElement element, int index)
        {
            var creature = (Creature)_creatureListView.itemsSource[index];

            var portrait = element.Q<Image>();
            portrait.image = creature.Portrait ? creature.Portrait.texture : Texture2D.grayTexture;

            var nameLabel = element.Q<Label>();
            nameLabel.text = creature.CreatureName;

            var deleteButton = element.Q<Button>();
            deleteButton.clicked += () => RemoveCreature(creature.Id);
        }
    }
}
