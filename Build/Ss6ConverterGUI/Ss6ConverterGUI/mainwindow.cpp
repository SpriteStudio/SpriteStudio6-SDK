#include "mainwindow.h"
#include "ui_mainwindow.h"

QString execPathStr;    //実行しているコンバータGUIのパス
QString cnvOutputStr;   //コンバート結果
bool convert_exec = false;  //コンバート中か
bool convert_error = false;  //コンバートエラーが発生したか
int convet_index = 0;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    //フォームの部品にアクセスする場合はuiのメンバを経由する
    ui->setupUi(this);

    //ドラッグ＆ドロップを有効にする
    setAcceptDrops(true);

    cnvProcess = new QProcess(this);

#if 0 // for Qt4
    // プロセスが終了した時に finished シグナル発信
    connect(cnvProcess, SIGNAL(finished(int, QProcess::ExitStatus)), this, SLOT(processFinished(int, QProcess::ExitStatus )));
    // プロセスからエラー出力があって読み込み可能になったら readyReadStandardError シグナル発信
    connect(cnvProcess, SIGNAL(readyReadStandardError()), this, SLOT(processErrOutput()));
    connect(cnvProcess, SIGNAL(readAllStandardOutput()), this, SLOT(processStdOutput()));
#else
    QObject::connect(cnvProcess, static_cast<void(QProcess::*)(int, QProcess::ExitStatus)>(&QProcess::finished), this, &MainWindow::processFinished);
    QObject::connect(cnvProcess, &QProcess::readyReadStandardOutput, this, &MainWindow::processStdOutput);
    QObject::connect(cnvProcess, &QProcess::readyReadStandardError, this, &MainWindow::processErrOutput);
#endif

    //ウィンドウのタイトルをつける
    setWindowTitle("Ss6Converter GUI Ver1.1.3");

    //ウィンドウサイズ固定
    setFixedSize( QSize(734,465) );

    //Documentsのパスを取得
    data_path = QStandardPaths::writableLocation(QStandardPaths::DocumentsLocation);
    data_path += "/SpriteStudio/Ss6ConVerter";
    QDir dir;
    //設定ファイル保存用ディレクトリを作成
    dir.mkpath(data_path);

    //スタイルシートを読み込む
    QFile file(":/style.qss");
    file.open(QFile::ReadOnly);
    QString styleSheet = QString::fromLatin1(file.readAll());
    qApp->setStyleSheet(styleSheet);

    //初期化
    convert_exec = false;
    cnvOutputStr.clear();

    ui->type_comboBox->addItem("ssbp");
    ui->type_comboBox->addItem("json");
    ui->type_comboBox->addItem("ssfb");
#ifdef Q_OS_WIN32
    ui->type_comboBox->addItem("sspkg");
#endif

}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::setText_to_List(QStringList list)
{
    //実行ファイルのパスを保存
    execPathStr = list[0];

    if ( list.length() > 1 )
    {
        int i;
        for ( i = 1; i < list.length(); i++ )
        {
            QString dragFilePath;
            dragFilePath = list[i];
            if ( ( dragFilePath.endsWith(".sspj")) || ( dragFilePath.endsWith(".SSPJ")) )
            {
                ui->listWidget->addItem(dragFilePath);
            }
        }
    }
    loadConfig();
}

void MainWindow::loadConfig(void)
{
    QString fileName = data_path + "/config";
    QString str;
    QFile file(fileName);

    if (!file.open(QIODevice::ReadOnly))//読込のみでオープンできたかチェック
    {
        return;
    }

    int read_count = 0;
    QTextStream in(&file);
    while ( !in.atEnd() ) {
        str = in.readLine();//1行読込

        switch( read_count )
        {
        case 0:
            {
                int idx = str.toInt();
                ui->type_comboBox->setCurrentIndex(idx);
            }
            break;
        }

        read_count++;
    }
}
void MainWindow::saveConfig(void)
{
    QString fileName = data_path + "/config";
    QFile file(fileName);

    if (!file.open(QIODevice::WriteOnly))//書込みのみでオープンできたかチェック
    {
        return;
    }

    QTextStream out(&file);
    out << ui->type_comboBox->currentIndex() << Qt::endl;
}

void MainWindow::on_pushButton_exit_clicked()
{
    //アプリケーションの終了
    saveConfig();
    exit(0);
}

void MainWindow::dragEnterEvent(QDragEnterEvent *e)
{
    if(e->mimeData()->hasUrls())
    {
        e->acceptProposedAction();
    }
}

void MainWindow::dropEvent(QDropEvent *e)
{
    if(e->mimeData()->hasUrls())
    {
        QList<QUrl> urlList = e->mimeData()->urls();
        for(int i = 0; i < urlList.size(); i++)
        {
            //ドラッグしたファイルをリストに追加
            //.sspj以外は弾く
            QString dragFilePath;
            dragFilePath = urlList[i].toLocalFile();
            if ( ( dragFilePath.endsWith(".sspj")) || ( dragFilePath.endsWith(".SSPJ")) )
            {
                //同じ名前がリストにある場合は弾く
                bool addname = true;
                int j = 0;
                for ( j = 0; j < ui->listWidget->count(); j++ )
                {
                    QString fileName = ui->listWidget->item(j)->text();
                    if ( fileName == dragFilePath )
                    {
                        addname = false;
                        break;
                    }

                }
                if ( addname == true )
                {
                    ui->listWidget->addItem(dragFilePath);
                }
            }
        }
    }
}

void MainWindow::on_pushButton_listclear_clicked()
{
    //リストクリア
    ui->listWidget->clear();
}

void MainWindow::on_pushButton_convert_clicked()
{
    //コンバート

    if (( ui->listWidget->count() > 0 ) && (convert_exec == false))
    {
        convert_error = false;
        convert_exec = false;  //コンバート中か
        convet_index = 0;
        QString st = QString("Exec %1/%2").arg(convet_index+1).arg(ui->listWidget->count());
        ui->textBrowser_status->setText(st);     //ステータス
        ui->textBrowser_err->setText(tr(""));           //エラー
        cnvOutputStr = "";

        convert_exec = true;  //コンバート中か
        saveConfig();
        int i;
        for ( i = 0; i < ui->listWidget->count(); i++ )
        {
            //進行状況表示
            QString st = QString("Exec %1/%2").arg(i+1).arg(ui->listWidget->count());
            ui->textBrowser_status->setText(st);     //ステータス

            //コンバート引数作成
            QString fileName = ui->listWidget->item(i)->text();
            //コンバータの起動
            if (fileName.isEmpty())
            {
                //ファイル名なし
            }
            else
            {
                QString execstr;

        #ifdef Q_OS_WIN32
                // Windows
                execstr = "Ss6Converter.exe";
        #else
                // Mac
                QDir appDir(QCoreApplication::applicationDirPath());
                QString commandPath = QDir(appDir).filePath("Ss6Converter");
                if (QFile(commandPath).exists()) {
                    execstr = commandPath;
                } else {
                    QDir dir = QDir(execPathStr);
                    dir.cd("..");
                    dir.cd("..");
                    dir.cd("..");
                    dir.cd("..");
                    QString str_current_path = dir.path();
                    execstr = str_current_path + "/Ss6Converter";
                }
        #endif

                QStringList args;
#ifdef Q_OS_WIN32
                //args.push_back("\"" + fileName + "\"");
                args.push_back(fileName);
#else
                args.push_back(fileName);
#endif
                args.push_back("-v");

                //オプション引数
                QString outType;
                if ( ui->type_comboBox->currentText() == "json" )
                {
                    outType = "json";
                }
                else if ( ui->type_comboBox->currentText() == "ssfb" )
                {
                    outType = "ssfb";
                }
                else if ( ui->type_comboBox->currentText() == "sspkg" )
                {
                    outType = "sspkg";
                }
                if (!outType.isEmpty())
                {
                    args.push_back("-f");
                    args.push_back(outType);
                }

                cnvProcess->start(execstr, args);

                if (cnvProcess->error() == QProcess::FailedToStart)
                {
                    ui->textBrowser_err->setText(cnvProcess->errorString() + execstr);
                    convert_error = true;
                    break;
                }

                button_enable(false);
                convert_exec = true;  //コンバート中か

                while ( 1 )
                {
                    QThread::sleep(1);  // Wait
                    QCoreApplication::processEvents();
                    if ( cnvProcess->state() != QProcess::Running )
                    {
//                        ui->textBrowser_status->setText(tr("Convert end"));
                        break;
                    }
                }
                QCoreApplication::processEvents();
                convet_index++;
            }
        }
        if ( convert_error == false )
        {
            ui->textBrowser_status->setText(tr("Convert Success!"));
        }
        else
        {
            ui->textBrowser_status->setText(tr("Error"));   //ステータス
        }
        button_enable( true );   //ボタン有効
        convert_exec = false;  //コンバート中か
    }
}

void MainWindow::processStdOutput()
{
    QByteArray output = cnvProcess->readAllStandardOutput();

    cnvOutputStr = cnvOutputStr + QString::fromLocal8Bit( output );
    ui->textBrowser_err->setText(cnvOutputStr);

    //カーソルを最終行へ移動
    QScrollBar *sb = ui->textBrowser_err->verticalScrollBar();
    sb->setValue(sb->maximum());

}

void MainWindow::processErrOutput()
{
    // 出力を全て取得
    QByteArray output = cnvProcess->readAllStandardError();

    cnvOutputStr = cnvOutputStr + QString::fromLocal8Bit( output );
    ui->textBrowser_err->setText(cnvOutputStr);

    //カーソルを最終行へ移動
    QScrollBar *sb = ui->textBrowser_err->verticalScrollBar();
    sb->setValue(sb->maximum());
}
void MainWindow::processFinished( int exitCode, QProcess::ExitStatus exitStatus)
{
    if ( exitStatus == QProcess::CrashExit )
    {
//        QMessageBox::warning( this, tr("Error"), tr("Crashed") );
        cnvOutputStr = cnvOutputStr + "Error:" + ui->listWidget->item(convet_index)->text();
        ui->textBrowser_err->setText(cnvOutputStr);
        convert_error = true;
        //カーソルを最終行へ移動
        QScrollBar *sb = ui->textBrowser_err->verticalScrollBar();
        sb->setValue(sb->maximum());
    }
    else if ( exitCode != 0 )
    {
//        QMessageBox::warning( this, tr("Error"), tr("Failed") );
        cnvOutputStr = cnvOutputStr + "Error:" + ui->listWidget->item(convet_index)->text();
        ui->textBrowser_err->setText(cnvOutputStr);
        convert_error = true;
        //カーソルを最終行へ移動
        QScrollBar *sb = ui->textBrowser_err->verticalScrollBar();
        sb->setValue(sb->maximum());
    }
    else
    {
        // 正常終了時の処理
//        ui->textBrowser_status->setText(tr("Convert Success!"));
//    QMessageBox::information(this, tr("Ss6Converter"), tr("Convert success"));
    }

}

//リストの読み込み
void MainWindow::on_pushButton_listload_clicked()
{
    QFileDialog::Options options;
    QString strSelectedFilter;
    QString fileName;
    fileName = QFileDialog::getOpenFileName(this, tr("select list File"), ".", tr("text(*.txt)"), &strSelectedFilter, options);

    if ( fileName != "" )
    {
        //リストクリア
        ui->listWidget->clear();

        //読み込んだファイルをリストに設定
        QFile file(fileName);

        if (!file.open(QIODevice::ReadOnly))//読込のみでオープンできたかチェック
        {
            return;
        }

        QTextStream in(&file);
        while ( !in.atEnd() ) {
            QString str = in.readLine();//1行読込
            ui->listWidget->addItem(str);
        }
    }

}

//リストの保存
void MainWindow::on_pushButton_listsave_clicked()
{
    QFileDialog::Options options;
    QString strSelectedFilter;
    QString fileName;
    fileName = QFileDialog::getSaveFileName(this, tr("save list File"), ".", tr("text(*.txt)"), &strSelectedFilter, options);

    if ( fileName != "" )
    {
        //読み込んだファイルをリストに設定
        QFile file(fileName);

        if (!file.open(QIODevice::WriteOnly))//読込のみでオープンできたかチェック
        {
            return;
        }

        QTextStream out(&file);
        int i;
        for ( i = 0; i < ui->listWidget->count(); i++ )
        {
            QString str = ui->listWidget->item(i)->text();
            out << str << Qt::endl; //書込み
        }
    }
}

void MainWindow::button_enable(bool flag)
{
    ui->pushButton_listsave->setEnabled(flag);
    ui->pushButton_listload->setEnabled(flag);
    ui->pushButton_listclear->setEnabled(flag);
    ui->pushButton_convert->setEnabled(flag);
}
