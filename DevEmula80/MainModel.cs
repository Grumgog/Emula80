using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows;
using System.Threading;
using Proc;
using Lexer;
using MessageBox = System.Windows.Forms.MessageBox;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace DevEmula80
{
    class MainModel : INotifyPropertyChanged
    {
        #region Данные не относящиеся к модели прямым способом
        private VirtualMachine VirtualMachine;
        private List<Token> Tokens;
        private int CommandPointer;
        ExecuteThread executeThread;
        #endregion

        #region Данные и их аксессоры
        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set{
                _FileName = value;
                OnPropertyChanged("FileName");
            }
        }
        private string _ProgramText;
        public string ProgramText
        {
            get => _ProgramText;
            set
            {
                _ProgramText = value;
                OnPropertyChanged("ProgramText");
                TextIsChanged = true;
                Tokens = null;
            }
        }
        private string _RunMessage;
        public string RunMessage
        {
            get => _RunMessage;
            set
            {
                _RunMessage = value;
                OnPropertyChanged("RunMessage");
            }
        }
        public ObservableCollection<int> MemoryState
        {
            get => VirtualMachine.proc.getFlatPresentOfMemory();
        }

        public ObservableCollection<Pair<string, int>> RegMem
        {
            get => VirtualMachine.proc.RegisterOfProcessor;
        }
        private bool _TextIsChanged;
        public bool TextIsChanged
        {
            get => _TextIsChanged;
            set
            {
                _TextIsChanged = value;
                OnPropertyChanged("TextIsChanged");
            }
        }
        private bool _NewDocumentState;
        public bool NewDocumentState
        {
            get => _NewDocumentState;
            set{ _NewDocumentState = value;  OnPropertyChanged("NewDocumentState"); }
        }

        private bool _AllExecute;
        public bool AllExecute
        {
            get => _AllExecute;
            set
            {
                _AllExecute = value;
                OnPropertyChanged("AllExecute");
            }
        }

        private bool _StepExecute;
        public bool StepExecute
        {
            get => _StepExecute;
            set
            {
                _StepExecute = value;
                OnPropertyChanged("StepExecute");
            }
        }
        #endregion

        #region Комманды и их аксессоры
        // Команды Текста
        private LocalCommand _NewDocument;
        public LocalCommand NewDocument
        {
            get => _NewDocument ?? (_NewDocument = new LocalCommand(obj =>
            {
                if (SaveQuestion())
                {
                    ProgramText = string.Empty;
                    FileName = "*";
                    NewDocumentState = true;
                    TextIsChanged = false;
                }
            }));
        }
        private LocalCommand _OpenFile;
        public LocalCommand OpenFile
        {
            get => _OpenFile ?? (_OpenFile = new LocalCommand(obj =>
            {
                if(!(NewDocumentState && !TextIsChanged)) SaveQuestion(); // Если документ не новый и текст изменен
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    DefaultExt = ".easm",
                    Filter = "Коды Emula (.easm)|*.easm",
                    CheckFileExists = true
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog.FileName);
                    ProgramText = reader.ReadToEnd();
                    reader.Close();
                    FileName = openFileDialog.FileName;
                }
            }));
        }
        private LocalCommand _SaveFile;
        public LocalCommand SaveFile
        {
            get => _SaveFile ?? (_SaveFile = new LocalCommand(obj =>
            {
                if (NewDocumentState)
                    _SaveFileAs.execute(null);
                else
                    SaveText();

            }));
        }
        private LocalCommand _SaveFileAs;
        public LocalCommand SaveFileAs
        {
            get => _SaveFileAs ?? (_SaveFileAs = new LocalCommand(obj =>
            {
                SaveTextWithDialog();
            }));
        }
        private LocalCommand _Print;
        public LocalCommand Print
        {
            get => _Print ?? (_Print = new LocalCommand(obj =>
            {
                
                PrintDialog printDialog = new PrintDialog();
                if(printDialog.ShowDialog() == true)
                {
                    FlowDocument fd = new FlowDocument(); // Печатаем потоковый документ
                    Paragraph mainBlock = new Paragraph(); // У нас один параграф
                    mainBlock.Margin = new Thickness(0); 
                    foreach (string line in ProgramText.Split('\n')) // Заполняем параграф строками
                    {
                        mainBlock.Inlines.Add(new Run(line));
                    }
                    fd.Blocks.Add(mainBlock);
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator; // получаем листы
                    printDialog.PrintDocument(paginator, FileName); // печатаем
                }
            }));
        }
        private LocalCommand _Exit;
        public LocalCommand Exit
        {
            get => _Exit ?? (_Exit = new LocalCommand(obj =>
            {
                if(SaveQuestion())
                Environment.Exit(0);
            }));
        }
        // Команды справки
        private LocalCommand _About;
        private LocalCommand _HelpSystem;
        // Команды исполнения
        private LocalCommand _RunAll;
        public LocalCommand RunAll
        {
            get => _RunAll ?? (_RunAll = new LocalCommand(obj =>
            {
                AllExecute = true;
                if(Tokens == null) // Если токенов не было или текст программы был изменен...
                {
                    Analizator analizator = new Analizator(new EmuDivider(), new EmuTokenizer());
                    Tokens = analizator.Process(ProgramText);
                    Tokens = Tokens.FindAll(el => el.Type != TokenType.NONE);
                } 
                try
                {
                    executeThread = new ExecuteThread(VirtualMachine, Tokens, this);
                }
                catch(Exception e)
                {
                   RunMessage = e.Message;
                    RunStop.execute(null);
                }
                AllExecute = false;
                OnPropertyChanged("RegMem");
                OnPropertyChanged("MemoryState");
            }, obj=> !StepExecute));
        }
        private LocalCommand _RunStep;
        public LocalCommand RunStep
        {
            get => _RunStep ?? (_RunStep = new LocalCommand(obj =>
            {
                StepExecute = true;
                
                if (Tokens == null)
                {
                    Analizator analizator = new Analizator(new EmuDivider(), new EmuTokenizer());
                    Tokens = analizator.Process(ProgramText);
                    Tokens = Tokens.FindAll(el => el.Type != TokenType.NONE);
                }
                try
                {
                    if (CommandPointer >= Tokens.Count)
                    {
                        RunStop.execute(null);
                        return;
                    }

                    RunMessage += Tokens[CommandPointer].ToString() + " выполняется\n";
                    CommandPointer = VirtualMachine.Step(Tokens, CommandPointer);
                    
                    OnPropertyChanged("RegMem");
                    OnPropertyChanged("MemoryState");
                }
                catch(Exception e)
                {
                    RunMessage += e.Message;
                    RunStop.execute(null);
                }
            }, obj=>!AllExecute));
        }
        private LocalCommand _RunStop;
        public LocalCommand RunStop
        {
            get => _RunStop ?? (_RunStop = new LocalCommand(obj =>
            {
                try
                {
                    if (AllExecute)
                        executeThread.thread.Abort();
                }
                catch
                {
                    RunMessage += "Работа программы была завершенна принудительно\n";
                }
                StepExecute = AllExecute = false;
                CommandPointer = 0;
                RunMessage += "Программа завершила выполнение\n";
            }, obj => StepExecute || AllExecute));
        }
        #endregion

        #region Вспомогательные функции
        private bool SaveQuestion()
        {
            if (TextIsChanged || NewDocumentState)
                if (MessageBox.Show("Сохранить текущий документ?", "Сохранение",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveFile_h();
                    return true;
                }
            return false;
        }

        private void SaveFile_h()
        {
            if (NewDocumentState)
                _SaveFileAs.execute(null);
            else
                _SaveFile.execute(null);
        }

        private void SaveText()
        {
            using (StreamWriter writer = new StreamWriter(FileName))
            {
                writer.WriteLine(ProgramText);
            }
            NewDocumentState = false; // Документ сохранен
        }

        private void SaveTextWithDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = ".easm",
                Filter = "Коды Emula (.easm)|*.easm"
            };
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                FileName = dialog.FileName;
                SaveText();
            }
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainModel()
        {
            VirtualMachine = new VirtualMachine(new Proc.Processor());
            _ProgramText = _RunMessage = "";
            FileName = "";
            NewDocumentState = true;
            CommandPointer = 0;
        }
    }

    public class LocalCommand : ICommand // класс для местных команд
    {
        public Action<object> execute;
        private Func<object, bool> canExecute;
        event EventHandler ICommand.CanExecuteChanged // Может ли применяться комманда (событие: изменение состояния)
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        bool ICommand.CanExecute(object parameter) // условия при котором может применяться команда
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public LocalCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        void ICommand.Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    class ExecuteThread
    {
        public Thread thread { get; set; }
        VirtualMachine vm;
        MainModel m;
        public ExecuteThread(VirtualMachine vm, List<Token> t, MainModel main)
        {
            this.vm = vm;
            thread = new Thread(this.func);
            thread.Name = "ExecuteAll";
            thread.Start(t);
            m = main;
        }

        void func(object info)
        {
            var s = info as List<Token>;
            try
            {
                App.Current.Dispatcher.Invoke(()=>vm.Execute(s));
            }
            catch(Exception e)
            {
                m.RunMessage += e.Message; 
                //m.AbortProgramm(); // <- Добавить код для возращение в исходное состояние
            }
           
            //vm.Execute(s);
        }

    }
}
