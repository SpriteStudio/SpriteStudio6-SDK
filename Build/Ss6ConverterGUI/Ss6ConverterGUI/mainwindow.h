#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QDesktopServices>
#include <QDir>
#include <QDropEvent>
#include <QFileDialog>
#include <QMainWindow>
#include <QMimeData>
#include <QProcess>
#include <QScrollBar>
#include <QStandardPaths>
#include <QTextStream>
#include <QThread>
#include <QUrl>

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow {
    Q_OBJECT

   public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();
    void setText_to_List(QStringList list);
    void dragEnterEvent(QDragEnterEvent *e);
    void dropEvent(QDropEvent *e);

   private slots:
    void on_pushButton_exit_clicked();

    void on_pushButton_listclear_clicked();

    void on_pushButton_convert_clicked();

   private:
    Ui::MainWindow *ui;
    QProcess *cnvProcess;
    void button_enable(bool flag);
    void loadConfig(void);
    void saveConfig(void);
    QString data_path;

   private slots:
    void processErrOutput();
    void processStdOutput();
    void processFinished(int exitCode, QProcess::ExitStatus exitStatus);
    void on_pushButton_listload_clicked();
    void on_pushButton_listsave_clicked();
};

#endif  // MAINWINDOW_H
